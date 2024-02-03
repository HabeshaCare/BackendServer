# Mobile App Integration Documentation:

## API Endpoint interaction

From the web API endpoints, the following are integrated into the mobile application:

- `/api/auth/signup`: Used for registering users. After signup, the user gets redirected to the login form.

- `/api/auth/login`: Used for user login. User information is cached in the mobile application for later reference in the profile section and subsequent information. The token is sent along with each request in the header to access protected routes.

- `/api/user/{id}`: Integrated to update a user’s profile. The mobile app uses this to update the user information, reflect the update on the profile page, and then update the cache of the user.

- `/api/user/{id}/chat`: Integrated to get the list of messages the user has conversed with the AI bot and also ask the AI bot using Get and Post methods respectively.

- `/api/doctor`: Integrated to see the list of doctors recommended for the user. The detailed profile information of each doctor is cached in the mobile app for later use.

- `/api/user/schedule`: Used to get a list of a user’s schedule in the mobile application. The necessary information of a user is found from the JWT token. The schedules are formatted and displayed in the mobile app. Users can also make a scheduled appointment with the doctor after visiting the detailed profile of a given doctor.
