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
    







