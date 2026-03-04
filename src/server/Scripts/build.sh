#!/bin/bash
if [ "$1" == "-r" ]
then
    rm -rf Build-out
    rm -rf wwwroot/media/*
fi

if [ ! -f "Build-out/DatabaseContextModelSnapshot.cs" ]
then
    dotnet ef Migrations add InitialCreate -o Build-out
fi
if [ ! -f "Build-out/DatabaseContext-bf7393b2-4072-46aa-96c7-05ac7563037b.db" ]
then
    dotnet ef database update
fi