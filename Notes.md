### Trust developement certificate
`
dotnet dev-certs https --trust
`

### Run docker
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
