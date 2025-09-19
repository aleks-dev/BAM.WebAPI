Requirement #	Description
SRS-1001	The application shall provide a Bank Account Management GUI that allows a user transfer money and make loan applications. The application will be for use by customers, not bank employees.
SRS-1002	The application shall have multiple users, each of which can have multiple accounts.
(No authentication is required; user can log in by entering their name.)
SRS-1003	The user class shall have (at least) the following properties:
•	User Name
•	Credit rating (Number from 1 - 100)
  Please add the following customers:
Name	Credit rating
Bob	15
Jim	45
Anne	80


SRS-1004	The account class shall have (at least) the following properties:
•	Balance
•	Account Type (Current, Savings, Loan)
SRS-1005	The Application shall allow a user to view their accounts and balances
SRS-1006	The Application shall allow a user to request a Loan
SRS-1007	A loan can have a duration 1, 3 or 5 years.
SRS-1008	A loan can have a maximum value of 10000.
SRS-1009	A loan application will be denied for users with credit rating < 20.
 
SRS-1010	The loan interest rate will be calculated as follows:
Credit rating	Duration	Interest rate
20-50	1	20
20-50	3	15
20-50	5	10
50-100	1	12
50-100	3	8
50-100	5	5

The interest calculation should be designed to be simple to extend and modify.
SRS-1011	A successful loan application shall increase the user’s account balance by the loan amount and shall create a loan account with the outstanding balance.
SRS-1012	The application will allow the user to transfer money between their accounts if there is enough money the account to do so.
