# WebAPI

All data types are `application/json` unless specified otherwise.

## Authentication methods used

The authentication method used in this API is JWT (Json Web Token). JWT is configured in the backend to contain the user information that is logged, including the user Id and role. This information is used later for different authorizations. The JWT is sent after login and should be used to make API calls to every protected route afterwards. The requests should set a header named `Authorization` with the value of `Bearer ` followed by the JWT token received for that session.

For authorization, there is a middleware configured to fetch some user information from the token for internal processes and also the .NET’s identity framework. This enables the protection of some routes based on the role of a certain user. The following demonstrates an example header used to get access to protected routes.

```json
{
  "headers": {
    "Authorization": "Bearer jwt_access_token"
  }
}
```

## Endpoints

### `/api/auth/login`

**Methods:** `Post`

**Body:**

```json
{
  "email": "user@example.com",
  "password": "string"
}
```

**Returned data:**

```json
{
  "token": "JWT_Token",
  "message": "Login successful",
  "user": {
    "id": "65bcc28f9e7d2e27a91cb82d",
    "profession": "string",
    "fullname": "Some one",
    "phonenumber": "0911926066",
    "city": "Dire Dawa",
    "age": 24,
    "imageUrl": "image_url",
    "role": "Admin",
    "email": "user@example.com",
    "gender": "Male"
  }
}
```

### `/api/auth/signup`

**Methods:** `Post`

**Body:**

```json
{
  "email": "user@example.com",
  "gender": "string",
  "phonenumber": "string",
  "profession": "string",
  "password": "string",
  "role": "Normal",
  "confirmPassword": "string"
}
```

**Returned data:**

```json
{
  "id": "65bcc28f9e7d2e27a91cb82d",
  "profession": "user’s profession",
  "fullname": "",
  "phonenumber": "091231223",
  "city": "city of user",
  "age": "age of user",
  "imageUrl": "image_url",
  "role": "Normal",
  "email": "user1@example.com",
  "gender": "Male"
}
```

### `/api/user/{id}`

**Methods:** `Put`

**Path:** id of the user to be updated

**Body:** any one or more of the following can be sent

```json
{
  "gender": "string",
  "email": "user@example.com",
  "profession": "string",
  "phonenumber": "string",
  "fullname": "string",
  "city": "string",
  "age": 150,
  "imageUrl": "string"
}
```

**Returned data:**

```json
{
  "message": "User updated successfully",
  "user": {
    "id": "65aa80034e3f9f40cfe50e13",
    "profession": "string",
    "fullname": "John Doe",
    "phonenumber": "0911926067",
    "city": "Addis Ababa",
    "age": 22,
    "imageUrl": "img_url",
    "role": "Admin",
    "email": "user18@example.com",
    "gender": "M"
  }
}
```

### `/api/user/{id}/picture`

**Methods:** `Post`

**Path:** id of the user to upload the image for

**Body:** the body is a multipart form with ‘image’ key set to the image that is uploaded. The image types supported are ‘jpg’ and ‘jpeg’ only.

**Returned data:**

```

```

Certainly! Here's the formatted content for the remaining section:

````markdown
### `/api/user/{id}/chat`

#### Methods

- **Get:**

  - Returns the list of messages found in from recent to old based on their createdDate.
  - **Path:** id of the user whose chat we want to retrieve
  - **Returned data:** The ‘type’ key in the returned data is an Enumeration where ‘0’ corresponds to ‘Human’ and ‘1’ corresponds to ‘Ai’ indicating whether the message was sent by a human or by the AI.

  ```json
  {
    "successMessage": "Found messages",
    "messages": [
      {
        "createdDate": "date and time",
        "content": "What are you?",
        "type": 0,
        "userId": "65b9679db44315585bdb9302"
      },
      {
        "createdDate": "date and time",
        "content": "I am a health assistant for patients, especially on stroke.",
        "type": 1,
        "userId": "65b9679db44315585bdb9302"
      }
    ]
  }
  ```
````

- **Post:**

  - **Path:** id of the user who is sending the message.
  - **Body:** the user’s message in string. It is a single string value that is going to be sent to the endpoint, not a JSON value. Eg. “Who are you?”.
  - **Returned data:** The returned response of the AI.

  ```json
  {
    "response": {
      "createdDate": "date and time",
      "content": "I am a health assistant for patients, especially on stroke. I can answer questions about stroke and provide resources to help patients and their families.",
      "type": 1,
      "userId": "65b9679db44315585bdb9302"
    },
    "message": "Asking llm successful"
  }
  ```

### `/api/doctor`

#### Methods

- **Get:**

  - **Query Parameters:** The parameters are optional, and the query will work without them.
    - MinYearExperience: int
    - MaxYearExperience: int
    - Specialization: string
    - Page: int (defaults to 1)
    - Size: int (defaults to 10)
  - **Returned Data:**

  ```json
  {
    "users": [
      {
        "licensePath": "path_to_licence information",
        "specialization": "Medical",
        "yearOfExperience": 1,
        "verified": true,
        "id": "65bcc2789e7d2e27a91cb82c",
        "profession": "Doctor",
        "fullname": "John doe",
        "phonenumber": "0913233423",
        "city": "Addis Ababa",
        "age": 23,
        "imageUrl": "img_url",
        "role": "Doctor",
        "email": "user2@example.com",
        "gender": "Male"
      }
    ]
  }
  ```

### `/api/doctor/{id}`

#### Methods

- **Get:**

  - **Path:** id of the doctor should be included in the path
  - **Returned data:**

  ```json
  {
    "user": {
      "licensePath": "path_to_license information",
      "specialization": "Medical",
      "yearOfExperience": 1,
      "verified": true,
      "id": "65bcc2789e7d2e27a91cb82c",
      "profession": "string",
      "fullname": "John doe",
      "phonenumber": "0923122334",
      "city": "Addis Ababa",
      "age": 23,
      "imageUrl": "img_url",
      "role": "Doctor",
      "email": "user2@example.com",
      "gender": "Male"
    }
  }
  ```

- **Put:**

  - **Path:** id of the doctor should be included in the path
  - **Body:** Any one or more of the following fields can be sent.

  ```json
  {
    "gender": "string",
    "email": "user@example.com",
    "profession": "string",
    "phonenumber": "string",
    "fullname": "string",
    "city": "string",
    "age": 150,
    "imageUrl": "string",
    "licensePath": "string",
    "specialization": "string",
    "yearOfExperience": 0
  }
  ```

  - **Response data:**

  ```json
  {
    "message": "Doctor profile updated successfully. Status set to unverified until approved by Admin",
    "user": {
      "licensePath": "path_to_license",
      "specialization": "Medical",
      "yearOfExperience": 0,
      "verified": false,
      "id": "65bcc2789e7d2e27a91cb82c",
      "profession": "Doctor",
      "fullname": "John Doe",
      "phonenumber": "0911926066",
      "city": "Addis Ababa",
      "age": 23,
      "imageUrl": "img_url",
      "role": "Doctor",
      "email": "user2@example.com",
      "gender": "Male"
    }
  }
  ```

Certainly! Here's the formatted content for the remaining section:

````markdown
### `/api/doctor/verify/{id}`

#### Methods

- **Put:**

  - **Path:** id of the doctor should be included in the path
  - **Returned data:**

  ```json
  {
    "message": "Doctor Verified",
    "success": true
  }
  ```
````

### `/api/doctor/{id}/license`

#### Methods

- **Post:**

  - **Path:** id included in the path
  - **Body:** multipart form of input ‘license’ and a .pdf file only.
  - **Returned data:**

  ```json
  {
    "message": "License Information Uploaded Successfully. Status set to unverified until approved by Admin",
    "user": {
      "licensePath": "path_to_license",
      "specialization": "Medical",
      "yearOfExperience": 0,
      "verified": false,
      "id": "65bcc2789e7d2e27a91cb82c",
      "profession": "string",
      "fullname": "",
      "phonenumber": "0911926066",
      "city": null,
      "age": null,
      "imageUrl": null,
      "role": "Doctor",
      "email": "user2@example.com",
      "gender": "Male"
    }
  }
  ```

### `/api/user/schedule`

The returned data sets the scheduler to null and doctor to its corresponding value if the schedule is being queried by the person who requested the schedule. On the contrary, if the doctor who is receiving the schedule is querying the API it will be returning the scheduler information instead of the doctor’s.

#### Method

- **Get:**

  - **Query parameters:** The following parameters are optional
    - Page: int with a default value of 1
    - Size: int with a default value of 10
  - **Returned data:**

  ```json
  {
    "message": "Found 1",
    "schedule": [
      {
        "id": "65bccab831105b0223e33e36",
        "scheduleTime": "stringified date time",
        "confirmed": false,
        "scheduler": null,
        "doctor": {
          "specialization": "Medical",
          "yearOfExperience": 1,
          "id": "65bcc2789e7d2e27a91cb82c",
          "fullname": "Yeabesera Derese",
          "gender": "Male",
          "phonenumber": "0911926066",
          "city": "Addis Ababa",
          "age": 23,
          "imageUrl": "img_url"
        }
      }
    ]
  }
  ```

- **Post:**

  - **Body:**

  ```json
  {
    "scheduleTime": "2024-02-03T09:22:15.481Z",
    "doctorId": "string"
  }
  ```

  - **Response data:**

  ```json
  {
    "message": "Schedule created successfully",
    "schedule": {
      "id": "65bccab831105b0223e33e36",
      "scheduleTime": "2024-01-19T12:36:52.748Z",
      "confirmed": false,
      "scheduler": null,
      "doctor": {
        "specialization": "",
        "yearOfExperience": 0,
        "id": "65bcc2789e7d2e27a91cb82c",
        "fullname": "",
        "gender": "Male",
        "phonenumber": "0911926066",
        "city": null,
        "age": null,
        "imageUrl": null
      }
    }
  }
  ```

### `/api/user/schedule/{id}`

#### Methods

- **Get:**

  - **Path:** id of the schedule to be fetched
  - **Returned Data:**

  ```json
  {
    "message": "Schedule Found",
    "schedule": {
      "id": "65bcc6ebcb7c13fb0956245e",
      "scheduleTime": "2024-01-19T12:36:52.748Z",
      "confirmed": false,
      "scheduler": null,
      "doctor": null
    }
  }
  ```

- **Put:**

  - **Path:** id of the schedule to be updated
  - **Body:** The data sent is just a string of the Date and time the schedule is going to be updated to. Eg. "2024-01-19T12:37:48.117Z"
  - **Response data:**

  ```json
  {
    "message": "Schedule Updated",
    "schedule": {
      "id": "65aa88d690206a778b158d39",
      "scheduleTime": "2024-01-19T12:37:48.117Z",
      "confirmed": true,
      "scheduler": {
        "id": "65aa6ef31f00917d5d488638",
        "fullname": "",
        "gender": "",
        "phonenumber": "0911926066",
        "city": null,
        "age": null,
        "imageUrl": null
      },
      "doctor": null
    }
  }
  ```

- **Delete:**

  - **Path:** id of the schedule that is going to be deleted
  - **Response data:**

  ```json
  {
    "message": "Schedule deleted successfully",
    "success": true
  }
  ```

### `/api/user/schedule/{scheduleId}/status`

#### Methods

- **Path:** id of the schedule whose status is going to be updated
- **Body:** a boolean value to set the status of the schedule. To update the “confirmed” field with. Send either ‘true’ or ‘false’ boolean.

```

```
