Feature: Email Address Verification

    Scenario: Email verification Page is displayed
        Given User navigates to the email address page
        Then they enter a valid email address
        When they click 'Continue'
        Then the email address verification page is displayed
        And there is an input field for the verification code
        And a "I have not received my passcode" link is displayed

    Scenario: Valid Passcode is entered
        Given User navigates to the email address page
        Then they enter a valid email address
        When they click 'Continue'
        Then the email address verification page is displayed
        And there is an input field for the verification code
        When they enter a valid passcode
        And they click 'Continue'
        And they are redirected to the enquiry question page

    Scenario: Attempting to continue without entering a passcode
        Given User navigates to the email address page
        Then they enter a valid email address
        When they click 'Continue'
        Then the email address verification page is displayed
        And there is an input field for the verification code
        When they click 'Continue'
        Then they will see 'Enter your passcode' as an error message for the 'no email adress'

    Scenario: Invalid Passcode is entered
        Given User navigates to the email address page
        Then they enter a valid email address
        When they click 'Continue'
        Then the email address verification page is displayed
        And there is an input field for the verification code
        When they enter an invalid passcode
        And they click 'Continue'
        Then they will see 'Enter a correct passcode' as an error message for the 'no email adress'

    Scenario: Attempt a passcode with letters and symbols
        Given User navigates to the email address page
        Then they enter a valid email address
        When they click 'Continue'
        Then the email address verification page is displayed
        And there is an input field for the verification code
        When they enter a passcode with letters and symbols
        And they click 'Continue'
        Then they will see 'The passcode must be a number' as an error message for the 'no email adress'

    Scenario: After entering a valid passcode email validation page disappears  from the journey for 60 minutes
        Given User navigates to the email address page
        Then they enter a valid email address
        When they click 'Continue'
        Then the email address verification page is displayed
        And there is an input field for the verification code
        When they enter a valid passcode
        And they click 'Continue'
        And they are redirected to the enquiry question page
        And they click 'Back'
        Then user is redirected to the enter email address page

    Scenario: User requests a new passcode
        Given User navigates to the email address page
        Then they enter a valid email address
        When they click 'Continue'
        Then the email address verification page is displayed
        And there is an input field for the verification code
        And a "I have not received my passcode" link is displayed
        When they click I have not received my passcode
        And a drop down menu is displayed with a request new passcode button
        When they click request new passcode


