Feature: Email address page of the enquiry builder

    Scenario: Email address page is after the guidance page
        Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'OX4 2AU'
        When they click 'Start now'
        And they click 'Continue' on enquiry
        And they will be taken to the single school selection page
        When the user clicks yes and continue
        Then user is redirected to the enter email address page
        And the page's title is 'We need to verify your school email address'

    Scenario: Going back returns to the guidance page
        Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'OX4 2AU'
        When they click 'Start now'
        And they click 'Continue' on enquiry
        And they will be taken to the single school selection page
        When the user clicks yes and continue
        And they click 'Back'
        Then they will be taken to the single school selection page

    Scenario: Invalid email address correct error to show
        Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'OX4 2AU'
        When they click 'Start now'
        And they click 'Continue' on enquiry
        And they will be taken to the single school selection page
        When the user clicks yes and continue
        Then they enter an invalid email address
        Then they click 'Continue'
        Then they will see the correct error message for an invalid email address

    Scenario: Valid email to navigate to the next page
        Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'OX4 2AU'
        When they click 'Start now'
        And they click 'Continue' on enquiry
        And they will be taken to the single school selection page
        When the user clicks yes and continue
        Then they enter a valid email address
        When they click 'Continue'
        Then the email address verification page is displayed
        When they click 'Back'
        Then user is redirected to the enter email address page

    Scenario: Clicking continue without an input error
        Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'OX4 2AU'
        When they click 'Start now'
        And they click 'Continue' on enquiry
        And they will be taken to the single school selection page
        When the user clicks yes and continue
        Then they click 'Continue'
        Then they will see 'Enter an email address' as an error message for the 'no email adress'


    Scenario: Data is saved when returning to email address page
        Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'OX4 2AU'
        When they click 'Start now'
        And they click 'Continue' on enquiry
        And they will be taken to the single school selection page
        When the user clicks yes and continue
        Then they enter a valid email address
        When they click 'Continue'
        Then the email address verification page is displayed
        When they click 'Back'
        Then user is redirected to the enter email address page
        Then the email address is visible in input field

    Scenario: user clicks service name on email address page
        Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'OX4 2AU'
        When they click 'Start now'
        And they click 'Continue' on enquiry
        And they will be taken to the single school selection page
        When the user clicks yes and continue
        When they click the 'Find a tuition partner' service name link
        Then they will be taken to the 'Find a tuition partner' journey start page

    Scenario: invalid email error redirect works as expected
        Given a user has started the 'Find a tuition partner' journey
        When user has journeyed forward to the check your answers page for an invalid email address
        Then the error message shows 'There was a problem sending the email and you should check the email address and try again'