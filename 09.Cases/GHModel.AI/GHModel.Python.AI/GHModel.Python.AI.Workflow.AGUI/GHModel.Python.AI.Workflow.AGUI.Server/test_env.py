"""Test if environment variables are loaded correctly."""
import os
from dotenv import load_dotenv

print("Current working directory:", os.getcwd())
print("\nTrying to load .env...")

# Try different paths
paths_to_try = [
    ".env",
    "workflow/.env",
    "../../../../../.env"
]

for path in paths_to_try:
    print(f"\nTrying: {path}")
    load_dotenv(path, override=True)
    
    github_endpoint = os.environ.get("GITHUB_ENDPOINT")
    github_token = os.environ.get("GITHUB_TOKEN")
    github_model = os.environ.get("GITHUB_MODEL_ID")
    
    print(f"  GITHUB_ENDPOINT: {github_endpoint}")
    print(f"  GITHUB_TOKEN: {'***' + github_token[-10:] if github_token else 'NOT SET'}")
    print(f"  GITHUB_MODEL_ID: {github_model}")
    
    if all([github_endpoint, github_token, github_model]):
        print(f"  ✅ All variables loaded from {path}")
        break
else:
    print("\n❌ Could not load all required environment variables from any path!")
