"""Test AG-UI integration directly."""

import asyncio
import os
from agent_framework_ag_ui import add_agent_framework_fastapi_endpoint
from workflow import workflow
from fastapi import FastAPI
from fastapi.testclient import TestClient

# Create test app
app = FastAPI(title="Test Travel Agent")

# Create agent from workflow
agent = workflow.as_agent(name="Travel Agent")

# Register endpoint
add_agent_framework_fastapi_endpoint(app, agent, "/")

# Test with TestClient
client = TestClient(app)

print("Testing AG-UI endpoint...")
print("=" * 60)

response = client.post(
    "/",
    json={"messages": [{"role": "user", "content": "I want to go to Paris"}]}
)

print(f"Status code: {response.status_code}")
print(f"Response headers: {response.headers}")
print(f"\nResponse text:")
print("-" * 60)
print(response.text)
print("-" * 60)

if response.text:
    print("\n✅ AG-UI endpoint is returning data!")
else:
    print("\n❌ AG-UI endpoint is NOT returning data!")
