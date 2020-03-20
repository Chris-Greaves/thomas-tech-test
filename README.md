# Thomas Technical Test

## API

### Candidate Endpoints

- Get Candidate by Id: Used to get a single candidate and their details including current status of assessments
- Get All Candidates: Gets all the candidates and their basic details
- Get All Candidates (with Pagination): Same as before but with the ability to set from and count
- Get All Candidates By Assessment: Gets all the Candidates that have been sent the specified assessment
- Candidate Search: Takes a string to compare against Candidate Names

GET /api/candidate/{id}
GET /api/candidate/all
GET /api/candidate/all/Assessment/{assessment}
GET /api/candidate/all/search/{searchString}
POST /api/candidate/all/page { from:int, num:int }