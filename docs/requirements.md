TechnicalAssessment \-{.NETDeveloper}\[III\] 

**Overview 1 Challenge 1 Requirements 2 Bonus (Optional) 3 Considerations 3 Deliverables 3** 

Overview 

● This technical assessment is to demonstrate your proficiency in .NET Web APIs and Web Development to create a full-stack web application. 

● The ideal applicant should possess a good understanding of ASP.NET Core, API development, database design, and frontend development using a single-page application framework/library. Use any single-page application framework/library you desire \- Angular would be preferred but is not required. 

● We ask that you complete everything in the requirements section. Items outlined in the “bonus” section are completely optional, and your evaluation will not be negatively affected if you choose not to complete them. 

Challenge 

Create a full-stack web application that will replicate the functionality of a local library that maintains an inventory of books. There is only one copy of each book. There are only two roles in this system; a librarian and a customer. There are certain actions that a customer and librarian can perform \- ensure the permissions in the API and front end reflect this. 

a. The librarian is responsible for managing books in the library. They can add books, edit books, remove books, or mark a book as returned from a customer. 

b. The customer can see all the books in the library, and there's no limit to the number of books they can borrow. A book is checked out for 5 days.  
![][image2]

Requirements 

1\. Build a frontend application 

a. Use Typescript 

b. Use a single-page application framework/library (Angular, React, Vue.js, etc.) c. Use any libraries you desire for development, UI/UX, etc. 

d. Structure and style the web application to your liking using best practices e. Create the design and user experience you would expect of a simple online library f. Support the following features, 

i. Users 

1\. Users should be able to sign up, log in, and log out. 

a. During sign-up, allow the user to specify if they are a 

Librarian or a Customer. 

ii. Featured Books 

1\. Display a list of random books that includes the Title, Author, 

Description, Cover Image, and Average User Rating of each book. 

2\. Users can filter and sort books by Title, Author, and Availability. 

iii. View Book Details 

1\. When a user selects a book, they should view the book's complete 

details including new fields such as Publisher, Publication Date, 

Category, ISBN, Page Count, and Customer Reviews. 

iv. Search For Books 

1\. Implement a search functionality that allows users to search for 

books by text that is partially contained in the book’s title. 

v. Manage Books 

1\. A librarian can add a new book, edit an existing book, or remove a 

book from the library. 

vi. Book Checkout 

1\. A customer can check out a book if it is available 

a. A book is checked out for 5 days 

b. There is only one copy of each book in this library 

2\. Only a librarian can mark a book as returned to the library 

vii. Customer Reviews 

1\. A customer can leave a review for a book that consists of a short 

message and a rating from 1-5 stars. 

2\. Create a .NET Web API using .NET 8+ 

a. Only users that are logged in are permitted to execute actions against the API b. API Controllers and routes should support all functionality needed for the application 

c. Configure Swagger UI / OpenAPI documentation for the API 

d. Use ASP.NET Identity for User/Role Management  
![][image3]

e. Use an ORM with code-first database migrations 

f. On Start, 

i. The database should be created and migrations applied 

ii. The database should be seeded with Books 

1\. Seed your database with data using Bogus for .NET 

3\. Use SQL Server / SQL Server Express 

a. Ensure the Book table includes, 

i. Title, Author, Description, Cover Image, Publisher, Publication Date, 

Category, ISBN, Page Count 

1\. Include any other relevant columns as you see fit 

Bonus (Optional) 

1\. Add the ability for a Librarian to view all checked-out books and their due dates 2\. Use a component library on the frontend 

3\. Add a unit test for the API 

4\. Create a database diagram 

5\. Implement a feature using SignalR 

Considerations 

● Any elements not specifically detailed in the guidelines are left to your discretion, and you are encouraged to use your sound judgment in completing the assessment as you see fit. ● Approach this exercise like any other development task, ensuring the code you produce is clean, robust, and ready for a production environment. 

● You should follow the standards, conventions, and best practices associated with any frameworks that you use. 

● It's important to integrate thoughtful design principles into your work. Be ready to explain the rationale behind your design decisions and the specifics of your implementations. ● The exercise is not timed and you are not rewarded for a fast turnaround, however, **we ask that you return it within (7) days from today's date**. If unforeseen situations demand more time, please let us know your requested delivery date. Failure to deliver on the requested date may result in disqualification. 

Deliverables 

● When complete, respond to this email with a link to your GitHub repository. ● Please ensure that your project includes a README.md file, containing any necessary setup or installation instructions that are not standard for the technology used in this assessment.



