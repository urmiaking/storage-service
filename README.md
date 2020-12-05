# storage-service
A Simple restful API for Upload and Download files by users implemented in ASP.NET Core 5 Web API.

# Instruction

1- Register an account by sending a POST request to api/Account/Register and fill in the username, password and confirmPassword in the request body. Now you have an account with 100MB Free storage.<br/>
2- Log into your account by sending a GET request to api/Account/Login and fill in the username and password query parameters. If the credentials entered correctly, then a token will generated. keep it. <br/>
3- Copy your token into Authorization header and set it's type to Bearer token. <br/>
4- Now you can try to upload your files using a post request to api/Files/Upload and specify file and isPublic keys in form-data. <br/>
5- You can see all downloadable files with api/Files/List url. and you can see your own files with api/Account/MyFiles. note that private files can't be downloaded by unauthorized users. <br/>

# Notes
1- Range download is supported.<br/>
2- Authentication is done by jwt token and Identity Platform.<br/>
3- You can see all apis in swagger using /swagger url
