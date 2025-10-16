# CSDL7

This is a startup template to create microservice based solutions.

## Before Running the Solution

### Generate Signing-Certificate for AuthServer 

#### Installing mkcert
This guide will be using `mkcert` for creating self-signed certificates. If it is not installed in your system, follow the [installation guide](https://github.com/FiloSottile/mkcert#installation) to install mkcert.

Then use the command to create root (local) certificate authority for your certificates:
```powershell
mkcert -install
```

#### Generate Signing-Certificate

Navigate to `/apps/auth-server/CSDL7.AuthServer` folder and run:

```bash
dotnet dev-certs https -v -ep ./openiddict.pfx -p 0b4481a6-2df1-41cc-aaf3-a0074b87d649
```

to generate pfx file for signing tokens by AuthServer.

> This should be done by every developer.

### Install Client-Side Libraries

Run the following command in this folder:

````bash
abp install-libs
````



### Deploying the application

Deploying an ABP application follows the same process as deploying any .NET or ASP.NET Core application. However, there are important considerations to keep in mind. For detailed guidance, refer to ABP's [deployment documentation](https://abp.io/docs/latest/deployment/distributed-microservice).

### Additional resources

#### Internal Resources

You can find detailed setup and configuration guide(s) for your solution below:

* [Docker-Compose for Infrastructure Dependencies](./etc/docker/README.md)

#### External Resources

You can see the following resources to learn more about your solution and the ABP Framework:

* [Microservice Development Tutorial](https://abp.io/docs/latest/tutorials/microservice)
* [Microservice Solution Template](https://abp.io/docs/latest/solution-templates/microservice)

