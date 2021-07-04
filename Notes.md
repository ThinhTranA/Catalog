### Trust developement certificate
`
dotnet dev-certs https --trust
`

### Run docker  
Create  
`
docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo
`  
(will take a while when this command run the first time, about 140MB)    

--rm: destroy container when our process is closed  
--name: give image a name so we can recognise it    
-p : open a port, mongodb usually listen in port 27017
(localport 27017 : 27017 mongdb port)   
-v : specify a volume so we don't lose data once we stopped 
the container

List running images     
`
docker ps   
`   
Stop running image  
`
docker stop mongo
`   
List store data/volume in docker    
`
docker volume ls
`   
Delete volume by name   
`
docker volume rm mongodbdata
`   

Create with auth    
`
docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db 
-e MONGO_INITDB_ROOT_USERNAME=mongoadmin
-e MONGO_INITDB_ROOT_PASSWORD=Pass#word1 
mongo
`   
(https://stackoverflow.com/questions/42912755/how-to-create-a-db-for-mongodb-container-on-start-up)

Stop running container:
`
docker stop mongo
`   
    
Notes: occasionally already running mongo (no auth) will not stop, and will require a manual process kill
for the old instance to stop. To check
`
pgrep mongo
`
if there is any number (eg: 503) meaning the old one still running, need to kill it
`kill 503`
    

### Dotnet secret manager
Init:   
`
dotnet user-secrets init
`   
Set password:   
`
dotnet user-secrets set MongoDbSettings:Password Pass#word1
`   
Password should now be picked up with just a field from `MongoDbSettings.cs` class 
without needing it in the `appsettings.json`    

### Build Docker image
name of `catalog` with tag `v1` and `.`(current directory)		

`
docker build -t catalog:v1 .
`
List created image: `docker images`


### Create docker network
Create a network with name `catalognetwork`
`
docker network create catalognetwork
`   	
List: 
`
docker network ls
`	
Join the mongo container with catalog container in the same network:    
`
docker stop mongo
`   
`
docker run -d --rm --name mongo -p 27017:27017 
-v mongodbdata:/data/db 
-e MONGO_INITDB_ROOT_USERNAME=mongoadmin 
-e MONGO_INITDB_ROOT_PASSWORD=Pass#word1 
--network=catalognetwork
mongo
`   
#### Run our image
`
docker run -it --rm -p 8080:80 
-e MongoDbSettings:Host=mongo 
-e MongoDbSettings:Password=Pass#word1
--network=catalognetwork
catalog:v1
`   
keep our terminal connect to the process `-it` (interactive)        
delete our container when we stop it `-rm`    
map port 8080 from local machine into port 80 (asp.netcore port) the container  
`-e` enviroment variable in docker to overwrite appsettings.json with enviroment variable
net work is `catalognetwork` and image is `catalog` with `v1` tag









