Feature: Enquiry Questions have correct structure and functionality

    Scenario: Tuition plan question shows correct error when no input provided
        Given user navigates to the first enquiry question
        When they click 'Continue'
        Then they will see 'Enter the type of tuition plan that you need' as an error message for the 'no input error'

    Scenario: Tuition plan question shows correct warning when reaching close to Max char limit
        Given user navigates to the first enquiry question
        When they type '7600' characters for question 1
        Then the warning should be displayed showing they have '2400' characters left
        When they type '11000' characters for question 1
        Then the warning should be displayed showing they are over by '1000' characters

    Scenario: Tuition plan second question shows correct warning when reaching close to Max char limit
        Given user navigates to the first enquiry question
        When they enter an answer for tuition plan
        And they click 'Continue'
        When they type '7600' characters for question 2
        Then the warning should be displayed showing they have '2400' characters left
        When they type '11000' characters for question 2
        Then the warning should be displayed showing they are over by '1000' characters

    Scenario: Tuition plan third question shows correct warning when reaching close to Max char limit
        Given user navigates to the first enquiry question
        When they enter an answer for tuition plan
        And they click 'Continue'
        Then they enter an answer for SEND requirements
        And they click 'Continue'
        When they type '7600' characters for question 3
        Then the warning should be displayed showing they have '2400' characters left
        When they type '11000' characters for question 3
        Then the warning should be displayed showing they are over by '1000' characters

    Scenario: User can skip second and third question in Enquiry Builder and will show as Not specified on check your answers
        Given user navigates to the first enquiry question
        When they enter an answer for tuition plan
        And they click 'Continue'
        Then they click 'Continue'
        And they click 'Continue'
        Then the text by the second and third questions is 'Not specified'


    Scenario: Back button works as expected for the first enquiry question
        Given user navigates to the first enquiry question
        When they click 'Back'
        Then user is redirected to the enter email address page

    Scenario: Back button works as expected for the second enquiry question
        Given user navigates to the first enquiry question
        When they enter an answer for tuition plan
        And they click 'Continue'
        When they click 'Back'
        Then they are redirected to the enquiry question page

    Scenario: Back button works as expected for the third enquiry question
        Given user navigates to the first enquiry question
        When they enter an answer for tuition plan
        And they click 'Continue'
        And they enter an answer for SEND requirements
        And they click 'Continue'
        And they click 'Continue'
        Then they click 'Back'
        Then they are redirected to the other requirements page


    Scenario: User clicks service name on enquiry questions
        Given user navigates to the first enquiry question
        When they click the 'Find a tuition partner' service name link
        Then they will be taken to the 'Find a tuition partner' journey start page
        When user navigates to the first enquiry question
        And they enter an answer for tuition plan
        And they click 'Continue'
        When they click the 'Find a tuition partner' service name link
        Then they will be taken to the 'Find a tuition partner' journey start page
        When user navigates to the first enquiry question
        And they enter an answer for tuition plan
        And they click 'Continue'
        Then they enter an answer for SEND requirements
        And they click 'Continue'
        When they click the 'Find a tuition partner' service name link
        Then they will be taken to the 'Find a tuition partner' journey start page


    Scenario: Clearing user session on question two of enquiry clears data saved and session timeout page is shown
        Given user navigates to the first enquiry question
        And they enter an answer for tuition plan
        Then they click 'Continue'
        When the user clears the current sessions data
        And they click 'Back'
        Then the session timeout page is shown

    Scenario: Clearing user session on question three of enquiry clears data saved and session timeout page is shown
        Given user navigates to the first enquiry question
        And they enter an answer for tuition plan
        Then they click 'Continue'
        Then they click 'Continue'
        When the user clears the current sessions data
        And they click 'Back'
        Then the session timeout page is shown
