import asyncio
from dotenv import load_dotenv
from workflow import workflow  # 🏗️ The content workflow



from agent_framework.observability import configure_otel_providers, get_tracer
from opentelemetry.trace import SpanKind
from opentelemetry.trace.span import format_trace_id
from agent_framework import setup_logging, WorkflowEvent,WorkflowBuilder,WorkflowContext,WorkflowOutputEvent




# Load environment variables first, before importing agents
load_dotenv()
setup_logging()

class DatabaseEvent(WorkflowEvent): ...


async def main():


    configure_otel_providers()

    print("Enter a prompt and the workflow will respond; type 'exit' to quit.")
    while True:
        prompt = input("\nYou: ").strip()
        if not prompt:
            continue
        if prompt.lower() in {"exit", "quit"}:
            print("Goodbye!")
            break

        result = ''

        
        with get_tracer().start_as_current_span("Sequential Workflow Scenario", kind=SpanKind.CLIENT) as current_span:
            print(f"Trace ID: {format_trace_id(current_span.get_span_context().trace_id)}")
            

            output_event = None

            async for event in workflow.run_stream(prompt):
                if isinstance(event, DatabaseEvent):
                    print(f"{event}")
                if isinstance(event, WorkflowOutputEvent):
                    output_event = event
                if isinstance(event, WorkflowEvent):
                    result += str(event.data)

            if output_event:
                print(f"Workflow completed with result: '{output_event.data}'")

            

        result = result.replace("None", "")

        print(f"\nAssistant:\n{result}\n")

            # result = result.replace("None", "")

            # print(f"\nAssistant:\n{result}\n")

    


if __name__ == "__main__":
    asyncio.run(main())