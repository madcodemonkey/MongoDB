# Summary
This folder contains a docker compose file to create a local instance of SQL Server (Linux version) that I can attach to.

# Requirements
Docker Desktop is installed.

# Usage
At a command prompt inside this directory, use one of these two ways

## Way 1: It takes control of the prompt till your done
Start command: 
```docker compose up```

Stop command:
Use CTRL-C, which will stop it.

## Way 2: It frees the prompt immediately and runs as a deamon 
Start command: 
```docker compose up -d```

Stop command:
```docker compose down```

# Compass Tool
If you want to look into the MongoDB running in the docker container, you can [download MongoDB's free tool called Compass](https://www.mongodb.com/try/download/compass).  

The connection string you will need to connect is ```mongodb://root:example@localhost:27017/?authMechanism=DEFAULT```

