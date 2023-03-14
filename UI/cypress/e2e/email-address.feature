Feature: Email address page of the enquiry builder

Scenario: Email address page is after the guidance page
Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
When user clicks 'Make an enquiry'
And they click 'Continue' on enquiry
Then user is redirected to the enter email address page
And the page's title is 'What is your email address?'

Scenario: Going back returns to the guidance page
Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
When user clicks 'Make an enquiry'
And they click 'Continue' on enquiry
And they click 'Back'
Then the user will navigate to the guidance page

Scenario: Invalid email address correct error to show
Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
When user clicks 'Make an enquiry'
And they click 'Continue' on enquiry
Then they enter an invalid email address
Then they click 'Continue'
Then they will see the correct error message for an invalid email address

Scenario: Valid email to navigate to the next page
Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
When user clicks 'Make an enquiry'
And they click 'Continue' on enquiry
Then they enter an valid email address
When they click 'Continue'
Then they are redirected to the enquiry question page
When they click 'Back'
Then user is redirected to the enter email address page

Scenario: Clicking continue without an input error
Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
When user clicks 'Make an enquiry'
And they click 'Continue' on enquiry
Then they click 'Continue'
Then they will see 'Enter an email address' as an error message for the 'no email adress'


Scenario: Data is saved when returning to email address page
Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
When user clicks 'Make an enquiry'
And they click 'Continue' on enquiry
Then they enter an valid email address
When they click 'Continue'
Then they are redirected to the enquiry question page
When they click 'Back'
Then user is redirected to the enter email address page
Then the email address is visible in input field

Scenario: user clicks service name on email address page
Given a user has arrived on the 'Which key stages' page
When they click the 'Find a tuition partner' service name link
Then they will be taken to the 'Find a tuition partner' journey start page