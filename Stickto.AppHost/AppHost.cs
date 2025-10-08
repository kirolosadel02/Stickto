var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Stickto>("stickto");

builder.AddProject<Projects.Stickto_Modules_CartService>("stickto-modules-cartservice");

builder.AddProject<Projects.Stickto_Modules_OrderService>("stickto-modules-orderservice");

builder.AddProject<Projects.Stickto_Modules_PaymentService>("stickto-modules-paymentservice");

builder.AddProject<Projects.Stickto_Modules_ProductService>("stickto-modules-productservice");

builder.AddProject<Projects.Stickto_Modules_UserService>("stickto-modules-userservice");

builder.AddProject<Projects.Stickto_Shared_Abstractions>("stickto-shared-abstractions");

builder.AddProject<Projects.Stickto_Shared_Infrastructure>("stickto-shared-infrastructure");

builder.Build().Run();
