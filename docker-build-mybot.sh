#!/bin/bash

export DOCKER_TAG="sorbra/dfdemo:1.0"

# Build and tag the docker container
docker build -t "sorbra/dfdemo:1.0" -f ./Dockerfile .

# Push container to Docker Hub
docker push "sorbra/dfdemo:1.0"
