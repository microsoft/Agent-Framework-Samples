import os
import httpx
import requests
from typing import List, Dict, Any, Union, Optional
from markdownify import markdownify
from dotenv import load_dotenv

load_dotenv()

def fetch_raw_content(url: str) -> Optional[str]:
    """
    Fetch HTML content from a URL and convert it to markdown format.
    
    Uses a 10-second timeout to avoid hanging on slow sites or large pages.
    
    Args:
        url: The URL to fetch content from
        
    Returns:
        The fetched content converted to markdown if successful,
        None if any error occurs during fetching or conversion
    """
    try:
        # Create a client with reasonable timeout
        with httpx.Client(timeout=10.0) as client:
            response = client.get(url)
            response.raise_for_status()
            return markdownify(response.text)
    except Exception as e:
        print(f"Warning: Failed to fetch full page content for {url}: {str(e)}")
        return None


def web_search(
    query: str, 
    max_results: int = 3, 
    fetch_full_page: bool = False,
    engines: Union[str, List[str]] = "google"
) -> List[Dict[str, Any]]:
    """
    Perform search using SerpAPI with support for Google and Baidu engines
    
    Args:
        query: Search query string
        max_results: Maximum number of results to return per engine
        fetch_full_page: Whether to fetch full page content
        engines: Search engine(s) to use. Can be "google", "baidu", or ["google", "baidu"]
        
    Returns:
        List of search results, each containing title, link, snippet, source engine, etc.
    """
    # Get SerpAPI key from environment variable
    api_key = os.getenv("SERPAPI_API_KEY")
    if not api_key:
        raise ValueError("Please set SERPAPI_API_KEY environment variable")
    
    # Normalize engines to list
    if isinstance(engines, str):
        engines = [engines]
    
    # Validate engines
    valid_engines = {"google", "baidu"}
    for engine in engines:
        if engine not in valid_engines:
            raise ValueError(f"Invalid engine: {engine}. Must be one of {valid_engines}")
    
    all_results = []
    
    # Search with each engine
    for engine in engines:
        try:
            results = _search_with_engine(
                query=query,
                engine=engine,
                max_results=max_results,
                api_key=api_key,
                fetch_full_page=fetch_full_page
            )
            all_results.extend(results)
        except Exception as e:
            print(f"Warning: Search with {engine} failed: {str(e)}")
            continue
    
    return all_results


def _search_with_engine(
    query: str,
    engine: str,
    max_results: int,
    api_key: str,
    fetch_full_page: bool
) -> List[Dict[str, Any]]:
    """
    Perform search with a specific engine
    
    Args:
        query: Search query string
        engine: Search engine to use ("google" or "baidu")
        max_results: Maximum number of results to return
        api_key: SerpAPI API key
        fetch_full_page: Whether to fetch full page content
        
    Returns:
        List of search results from the specified engine
    """
    # SerpAPI request parameters
    params = {
        "engine": engine,
        "q": query,
        "api_key": api_key,
    }
    
    # Add engine-specific parameters
    if engine == "google":
        params["num"] = max_results  # Limit number of results for Google
    elif engine == "baidu":
        params["num"] = max_results  # Baidu also supports num parameter
    
    try:
        # Send request to SerpAPI
        response = requests.get("https://serpapi.com/search", params=params)
        response.raise_for_status()
        data = response.json()
        
        # Parse search results
        results = []
        organic_results = data.get("organic_results", [])
        
        for result in organic_results[:max_results]:
            url = result.get("link", "")
            title = result.get("title", "")
            content = result.get("snippet", "")
            
            # Skip incomplete results
            if not all([url, title, content]):
                print(f"Warning: Incomplete result from {engine}: {result}")
                continue
            
            # Fetch full page content if needed
            raw_content = content
            if fetch_full_page:
                raw_content = fetch_raw_content(url)
            
            # Add result to list
            search_result = {
                "title": title,
                "url": url,
                "content": content,
                "raw_content": raw_content,
                "position": result.get("position", 0),
                "source_engine": engine,
            }
            
            results.append(search_result)
        
        return results
        
    except requests.exceptions.RequestException as e:
        print(f"Error in {engine} search: {str(e)}")
        print(f"Full error details: {type(e).__name__}")
        raise Exception(f"SerpAPI request failed for {engine}: {str(e)}")
    except Exception as e:
        print(f"Error in {engine} search: {str(e)}")
        print(f"Full error details: {type(e).__name__}")
        raise Exception(f"Error occurred during {engine} search: {str(e)}")
