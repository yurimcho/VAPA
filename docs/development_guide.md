# Git 명령어 정리
## github와 연결
~~~~
# git remote add origin github 저장소 url
~~~~
## github 저장소 복사
~~~~
# git clone github 저장소 url
~~~~


# Conda 명령어 정리

## Create a conda environment
conda create --name <environment-name> python=<version:2.7/3.5>

## To create a requirements.txt file:
conda list #Gives you list of packages used for the environment

conda list -e > requirements.txt #Save all the info about packages to your folder

## To export environment file
activate <environment-name>
conda env export > <environment-name>.yml

## For other person to use the environment
conda env create -f <environment-name>.yml
