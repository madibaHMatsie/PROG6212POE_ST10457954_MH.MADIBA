# RaceDay Platform 
## Setup Instructions
1. Open the solution in visual studio.
2.  Ensure the API project is set as a startup project and run it.
3.  Run the MVC project

## Docker Instructions 
   To run the MVC application in a Docker container, use the following commands:
   \`\`\`bash
docker build -t raceday-mvc -f RaceDayMVC/Dockerfile .
docker run -d -p 8080:80 raceday-mvc
\`\`\`
