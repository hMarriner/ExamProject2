![](http://images.restapi.co.za/pvt/Noroff-64.png)

# Noroff

# Back-end Development Year 2

### Exam Project 2

This repository does not have any startup code. Use the 2 folders

- Backend
- Frontend

for your respective applications.

Instruction for the course assignment is in the LMS (Moodle) system of Noroff.
[https://lms.noroff.no](https://lms.noroff.no)

![](http://images.restapi.co.za/pvt/ca_important.png)

You will not be able to make any submissions after the course assignment deadline. Make sure to make all your commit **BEFORE** the deadline to this repository.

![](http://images.restapi.co.za/pvt/help.png)

If you need help with any instructions for the course assignment, contact your teacher on **Microsoft Teams**.

**REMEMBER** Your Moodle LMS submission must have your repository link **AND** your Github username in the text file.

# Clinic Appointment Booking API

This API manages clinics, doctors, appointment categories, patient accounts, and appointment bookings.

## Base URL

- API (HTTP): http://localhost:5054/api
- API (HTTPS): https://localhost:7266/api
- Swagger docs: http://localhost:5054/doc

## Frontend URL

- Default: http://localhost:3000

### ENDPOINTS

#### Auth

| Method | Endpoint           | Auth   | Description                |
| ------ | ------------------ | ------ | -------------------------- |
| POST   | /api/Auth/register | Public | Register a new patient     |
| POST   | /api/Auth/login    | Public | Log in and receive a token |

#### Patient

| Method | Endpoint        | Auth     | Description                  |
| ------ | --------------- | -------- | ---------------------------- |
| GET    | /api/Patient/me | Required | Get current patient profile  |
| PUT    | /api/Patient/me | Required | Edit current patient profile |
| DELETE | /api/Patient/me | Required | Delete current patient       |

#### Clinic

| Method | Endpoint         | Auth     | Description                    |
| ------ | ---------------- | -------- | ------------------------------ |
| GET    | /api/Clinic      | Public   | Get all clinics                |
| GET    | /api/Clinic/{id} | Public   | Get one clinic by id           |
| POST   | /api/Clinic      | Required | Create a clinic                |
| PUT    | /api/Clinic/{id} | Required | Update a specific clinic by id |
| DELETE | /api/Clinic/{id} | Required | Delete a specific clinic by id |

#### Doctor

| Method | Endpoint                     | Auth     | Description                    |
| ------ | ---------------------------- | -------- | ------------------------------ |
| GET    | /api/Doctor                  | Public   | Get all doctors                |
| GET    | /api/Doctor/{id}             | Public   | Get one doctor by id           |
| GET    | /api/Doctor/search?query=... | Public   | Search doctors by name         |
| POST   | /api/Doctor                  | Required | Create a doctor                |
| PUT    | /api/Doctor/{id}             | Required | Update a specific doctor by id |
| DELETE | /api/Doctor/{id}             | Required | Delete a specific doctor by id |

#### Appointment

| Method | Endpoint                       | Auth     | Description                                                               |
| ------ | ------------------------------ | -------- | ------------------------------------------------------------------------- |
| GET    | /api/Appointment               | Required | Get appointments for the current authenticated patient                    |
| GET    | /api/Appointment/{id}          | Required | Get specific appointment for the authenticated patient by id              |
| GET    | /api/Appointment/availability  | Public   | Get available time slots for appointments                                  |
| POST   | /api/Appointment               | Required | Create an appointment for the current authenticated patient               |
| PUT    | /api/Appointment/{id}          | Required | Update a specific appointment for the current authenticated patient by id |
| DELETE | /api/Appointment/{id}          | Required | Delete a specific appointment for the current authenticated patient by id |
| POST   | /api/Appointment/guest-booking | Public   | Create an appointment for a guest without an account                      |

#### Speciality

| Method | Endpoint             | Auth     | Description                        |
| ------ | -------------------- | -------- | ---------------------------------- |
| GET    | /api/Speciality      | Public   | Get all specialities               |
| GET    | /api/Speciality/{id} | Public   | Get one speciality by id           |
| POST   | /api/Speciality      | Required | Create a speciality                |
| PUT    | /api/Speciality/{id} | Required | Update a specific speciality by id |
| DELETE | /api/Speciality/{id} | Required | Delete a specific speciality by id |

#### Appointment Category

| Method | Endpoint                      | Auth     | Description                                  |
| ------ | ----------------------------- | -------- | -------------------------------------------- |
| GET    | /api/AppointmentCategory      | Public   | Get all appointment categories               |
| GET    | /api/AppointmentCategory/{id} | Public   | Get one appointment category by id           |
| POST   | /api/AppointmentCategory      | Required | Create an appointment category               |
| PUT    | /api/AppointmentCategory/{id} | Required | Update a specific appointment category by id |
| DELETE | /api/AppointmentCategory/{id} | Required | Delete a specific appointment category by id |

### EXTERNAL LIBRARIES

#### Backend (.NET 9)

- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.OpenApi
- Microsoft.EntityFrameworkCore.Design
- MySql.EntityFrameworkCore
- SwashBuckle.AspNetCore (Swagger)
- System.IdentityModel.Tokens.Jwt

#### Frontend (Node.js)

- next, react, react-dom
- tailwindcss, postcss, autoprefixer
- typescript
- eslint, eslint-confiq-next
- prettier
- babel-plugin-react-compiler

### HOW TO RUN THE APPLICATION

#### Prerequisites

- .NET 9 SDK
- Node.js (v20 or newer) and npm
- A running MySQL server

#### 1. Backend

1. Open a terminal in the `Backend` folder.
2. Set the database connection string and JWT secret with user secrets ( replace the values with your own):

```
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=appointment_bookingdb;Use
dotnet user-secrets set "Jwt:Secret" "YOUR_LONG_RANDOM_SECRET_AT_LEAST_32_CHARACTERS"
```

3. Create the database from the migrations (requires the EF Core CLI: `dotnet tool install --global dotnet-ef`):

```
dotnet ef database update
```

4. Start the API with the HTTPS profile:

```
dotnet run --launch-profile https
```

5. Open the Swagger documentation at http://localhost:5054/doc (or https://localhost:7266/doc).

Swagger is only available when `ASPNETCORE_ENVIRONMENT` is `Development`, which both launch profiles set.

#### 2. Frontend

1. Open a second terminal in the `Frontend` folder.
2. Install the dependencies:

```
npm install
```

3. The frontend calls `http://localhost:5054/api` by default from the .env.local file.

4. Start the frontend:

```
npm run dev
```

5. Open http://localhost:3000. The booking page is at `/`, with `/book`, `/search`, `/login`, `/register`, `/appointments` and `/myprofile`.

Start the backend brfore the frontend. The backend only accepts requests from `http://localhost:3000`.

### REFERENCES

During this Exam project i have used the following resources and tools:

- Noroff's Backend Technologies Moodle course and assignment.
- Noroff's Frontend Technologies 2 Moodle course and assignment.
- VS Code Copilot, used for function explanation and spelling and assist troubleshooting.
- Jira for project management.
