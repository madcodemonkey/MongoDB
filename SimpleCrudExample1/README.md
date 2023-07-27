Simple CRUD Example1

This example is based on the [YouTube video titled "How to Connect MongoDB to C# the Easy Way" by Tim Corey](https://www.youtube.com/watch?v=exXavNOqaVo).

# Setup
You'll need to run a MongoDB docker container to use this example.  You'll find a Docker Compose file at the root of the repository.  If you download it, just do a ```docker compose up -d``` to get it up and running.  Afterwards, you can run this example normally from Visual Studio.

# Compass Tool
If you want to look into the MongoDB running in the docker container, you can [download MongoDB's free tool called Compass](https://www.mongodb.com/try/download/compass).  

The connection string you will need to connect is ```mongodb://root:example@localhost:27017/?authMechanism=DEFAULT```



