Web API with MongoDB

# Setup
You'll need to run a MongoDB docker container to use this example.  You'll find a Docker Compose file at the root of the repository.  If you download it, just do a ```docker compose up -d``` to get it up and running.  Afterwards, you can run this example normally from Visual Studio.

OR

You can use a free MongoDB Atlas cluster.  Go to [start free](https://www.mongodb.com/cloud/atlas/register) page and register for your free account.  Afterwards, update mongoDbConnectionString in the appsettings.json file.


# Compass Tool
If you want to look into the MongoDB running in the docker container, you can [download MongoDB's free tool called Compass](https://www.mongodb.com/try/download/compass).  

The connection string you will need to connect is ```mongodb://root:example@localhost:27017/?authMechanism=DEFAULT```



