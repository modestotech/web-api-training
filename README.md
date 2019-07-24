# Web API Training
Poster containing the whole life cycle flow can be found at [this](https://www.asp.net/media/4071077/aspnet-web-api-poster.pdf) link.
## HTTP Message Life cycle
Opiniated explanation from course
### Delegating handlers
Can abort pipeline if basic required message elements are present. Routing has not happened at this stage
### Authentication Filters
Some routes might not need authentication, can be achieved here.
### Authorization Filters
Idem
### Action Filters
Logging, knowing which method that will be executed, last second validation or manipulation now that model binding has happened, or something related to a specific route, etc
### Examples
- If it's per route or needs the action context 			-> Action filter
- If it's a message layer processing needed for all routes 	-> Delegating handler
