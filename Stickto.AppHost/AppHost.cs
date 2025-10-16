var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject("stickto", "../Stickto/Stickto.csproj");

builder.AddProject("stickto-modules-cartservice", "../Stickto.Modules.CartService/Stickto.Modules.CartService.csproj");

builder.AddProject("stickto-modules-orderservice", "../Stickto.Modules.OrderService/Stickto.Modules.OrderService.csproj");

builder.AddProject("stickto-modules-paymentservice", "../Stickto.Modules.PaymentService/Stickto.Modules.PaymentService.csproj");

builder.AddProject("stickto-modules-productservice", "../Stickto.Modules.ProductService/Stickto.Modules.ProductService.csproj");

builder.AddProject("stickto-modules-userservice", "../Stickto.Modules.UserService/Stickto.Modules.UserService.csproj");

builder.AddProject("stickto-shared-abstractions", "../Stickto.Shared.Abstractions/Stickto.Shared.Abstractions.csproj");

builder.AddProject("stickto-shared-infrastructure", "../Stickto.Shared.Infrastructure/Stickto.Shared.Infrastructure.csproj");

builder.Build().Run();
