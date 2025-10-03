// See https://aka.ms/new-console-template for more information

using Dotnet.GprcService;
using Grpc.Core;
using Grpc.Net.Client;

Console.WriteLine("gRPC Client - Dynamic URL and Interactive Demo");
Console.WriteLine("----------------------------------------------------------");

string? serviceUrlInput;
bool isValidUrl = false;
Uri? serviceUri = null;

do
{
    Console.Write("Please enter the gRPC service URL (default: http://localhost:5296): ");
    serviceUrlInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(serviceUrlInput))
    {
        serviceUrlInput = "http://localhost:5296";
        Console.WriteLine($"Using default URL: {serviceUrlInput}");
    }

    if (Uri.TryCreate(serviceUrlInput, UriKind.Absolute, out serviceUri) &&
        (serviceUri.Scheme == Uri.UriSchemeHttp || serviceUri.Scheme == Uri.UriSchemeHttps))
    {
        isValidUrl = true;
    }
    else
    {
        Console.WriteLine("Invalid URL format. Please enter a valid HTTP or HTTPS URL.");
    }
} while (!isValidUrl);

Console.WriteLine($"Attempting to connect to: {serviceUri?.OriginalString}");

if (serviceUri?.Scheme == Uri.UriSchemeHttp)
{
    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
}

try
{
    using var channel = GrpcChannel.ForAddress(serviceUri?.OriginalString ??
                                               throw new InvalidOperationException("Service URL cannot be null."));
    Console.WriteLine("\n--- Calling Greeter Service ---");
    Console.Write("Enter your name for the greeting: ");
    string? nameInput = Console.ReadLine();
    nameInput = string.IsNullOrWhiteSpace(nameInput) ? "Anonymous" : nameInput;
    var greeterClient = new Greeter.GreeterClient(channel);
    var greeterReply = await greeterClient.SayHelloAsync(new HelloRequest { Name = nameInput });
    Console.WriteLine($"Greeting from {greeterReply.Message}");
}
catch (RpcException ex)
{
    Console.WriteLine($"{ex.Status.StatusCode} - {ex.Status.Detail}");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}