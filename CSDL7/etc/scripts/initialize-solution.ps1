abp install-libs
cd apps/auth-server/CSDL7.AuthServer && dotnet dev-certs https -v -ep openiddict.pfx -p 0b4481a6-2df1-41cc-aaf3-a0074b87d649 && cd -

exit 0