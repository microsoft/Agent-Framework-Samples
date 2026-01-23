"""Quick test client for single agent server."""

import asyncio
import os
from agent_framework_ag_ui import AGUIChatClient

async def test_server(port=8890):
    """Test AG-UI server at specified port."""
    server_url = f"http://127.0.0.1:{port}/"
    
    print(f"Testing server at {server_url}")
    print("=" * 60)
    
    async with AGUIChatClient(endpoint=server_url) as client:
        message = "I want to go to Paris"
        print(f"Sending: {message}\n")
        print("Response:")
        print("-" * 60)
        
        has_content = False
        async for update in client.get_streaming_response(message):
            if update.contents:
                for content in update.contents:
                    if hasattr(content, "text") and content.text:
                        print(content.text, end="", flush=True)
                        has_content = True
        
        print("\n" + "=" * 60)
        if has_content:
            print("✅ Server is working!")
        else:
            print("❌ Server returned no content!")

if __name__ == "__main__":
    import sys
    port = int(sys.argv[1]) if len(sys.argv) > 1 else 8890
    asyncio.run(test_server(port))
