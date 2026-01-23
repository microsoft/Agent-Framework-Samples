"""Complete test of agents and workflow."""

import asyncio
from workflow import workflow

async def test_workflow_output():
    """Test if workflow produces output."""
    print("=" * 60)
    print("Testing Workflow Output")
    print("=" * 60)
    
    test_message = "I want to go to Paris"
    print(f"\nSending message: {test_message}")
    print("\nWorkflow response:")
    print("-" * 60)
    
    response_parts = []
    event_count = 0
    
    async for event in workflow.run_stream(test_message):
        event_count += 1
        print(f"\n[Event {event_count}]")
        print(f"  Type: {type(event).__name__}")
        
        if hasattr(event, 'data'):
            data = str(event.data)
            response_parts.append(data)
            print(f"  Data: {data}")
        
        if hasattr(event, 'executor_id'):
            print(f"  Executor: {event.executor_id}")
        
        if hasattr(event, 'metadata'):
            print(f"  Metadata: {event.metadata}")
    
    print("\n" + "=" * 60)
    print(f"Total events: {event_count}")
    print(f"Total response parts: {len(response_parts)}")
    
    full_response = "".join(response_parts)
    print(f"Full response length: {len(full_response)}")
    print(f"\nFull response:\n{full_response}")
    print("=" * 60)
    
    if full_response:
        print("\n✅ Workflow is producing output!")
        return True
    else:
        print("\n❌ Workflow is NOT producing output!")
        return False

if __name__ == "__main__":
    result = asyncio.run(test_workflow_output())
    exit(0 if result else 1)
