Feature: Tuition Partner Enquiry Response Journey

    Scenario: Response page has correct url and headings
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner clicks the magic link to respond to a schools enquiry
        When the user has arrived on the tuition response page
        Then the page heading should show School Enquiry from 'Stockport' area
        Then the page should display You have '7' days to respond
        And the responses should have heading 'Respond to this enquiry'

    Scenario: Enquiry Questions have the expected data
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner has arrived on respond to an enquiry page
        When the user has arrived on the tuition response page
        And the first response section is to be Key stage and subjects
        Then the key stages and subjects should match the request:
            | Section Name            | Expected Content                                                |
            | Key stages and subjects | Displays Key Stages and subjects table with the following data: |
            |                         | Key stage 1: English and Maths                                  |
            |                         | Key stage 2: English and Maths                                  |
        And the second response section is to be 'Tuition type:' with Type 'Any'
        And the third response section is to be "Tuition plan:" with text 'enquiry'
        Then the fourth response section is to be 'SEND requirements:' with text 'enquiry'
        And the last response section is to be 'Other tuition considerations:' with text 'enquiry'

    Scenario: Clicking continue on successful tuition responses shows check your answers page
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner has arrived on respond to an enquiry page
        When the user has arrived on the tuition response page
        And they type '10' characters for section 1
        When they type '10' characters for section 2
        And they type '10' characters for section 3
        When they type '10' characters for section 4
        When they type '10' characters for section 5
        When they click 'Continue'
        Then the user has arrived on the tuition response check your answers page

    Scenario: Tuition responses show correct warning when reaching close to and over Max char limit for Key-stage and subjects
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner has arrived on respond to an enquiry page
        When the user has arrived on the tuition response page
        And they type '7700' characters for section 1
        Then the warning should be displayed showing they have '2300' characters left
        When they type '12100' characters for section 1
        Then the warning should be displayed showing they are over by '2100' characters
        When they type '80' characters for section 2
        When they type '80' characters for section 3
        When they type '80' characters for section 4
        When they type '80' characters for section 5
        When they click 'Continue'
        Then the error message shows 'Key stages and subjects must be 10000 characters or less'

    Scenario: Tuition responses show correct warning when reaching close to and over Max char limit for tuition type
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner has arrived on respond to an enquiry page
        When the user has arrived on the tuition response page
        And they type '70' characters for section 1
        When they type '8800' characters for section 2
        Then the warning should be displayed showing they have '1200' characters left
        When they type '14000' characters for section 2
        Then the warning should be displayed showing they are over by '4000' characters
        When they type '80' characters for section 3
        When they type '80' characters for section 4
        When they type '80' characters for section 5
        When they click 'Continue'
        Then the error message shows 'Tuition type must be 10000 characters or less'


    Scenario: Tuition responses show correct warning when reaching close to and over Max char limit for tuition plan
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner has arrived on respond to an enquiry page
        When the user has arrived on the tuition response page
        And they type '70' characters for section 1
        When they type '80' characters for section 2
        And they type '9900' characters for section 3
        Then the warning should be displayed showing they have '100' characters left
        When they type '10050' characters for section 3
        Then the warning should be displayed showing they are over by '50' characters
        When they type '80' characters for section 4
        When they type '80' characters for section 5
        When they click 'Continue'
        Then the error message shows 'Tuition plan must be 10000 characters or less'


    Scenario: Tuition responses show correct warning when reaching close to and over Max char limit for SEND requirements
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner has arrived on respond to an enquiry page
        When the user has arrived on the tuition response page
        And they type '70' characters for section 1
        When they type '80' characters for section 2
        And they type '90' characters for section 3
        When they type '7800' characters for section 4
        Then the warning should be displayed showing they have '2200' characters left
        When they type '12100' characters for section 4
        Then the warning should be displayed showing they are over by '2100' characters
        When they type '80' characters for section 5
        When they click 'Continue'
        Then the error message shows 'SEND requirements must be 10000 characters or less'


    Scenario: Tuition responses show correct warning when reaching close to and over Max char limit for Other school considerations
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner has arrived on respond to an enquiry page
        When the user has arrived on the tuition response page
        And they type '70' characters for section 1
        When they type '80' characters for section 2
        And they type '90' characters for section 3
        When they type '70' characters for section 4
        When they type '7700' characters for section 5
        Then the warning should be displayed showing they have '2300' characters left
        When they type '10002' characters for section 5
        Then the warning should be displayed showing they are over by '2' characters
        When they click 'Continue'
        Then the error message shows 'Other tuition considerations must be 10000 characters or less'

    Scenario: Tuition responses show correct warning when there is no input for Key-stage and subjects
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner has arrived on respond to an enquiry page
        When the user has arrived on the tuition response page
        And they type '0' characters for section 1
        When they type '80' characters for section 2
        And they type '90' characters for section 3
        When they type '70' characters for section 4
        When they type '70' characters for section 5
        When they click 'Continue'
        Then the error message shows 'Enter can you support those key stages and subjects'


    Scenario: Tuition responses show correct warning when there is no input for Tuition Type
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner has arrived on respond to an enquiry page
        When the user has arrived on the tuition response page
        And they type '10' characters for section 1
        When they type '0' characters for section 2
        And they type '90' characters for section 3
        When they type '70' characters for section 4
        When they type '70' characters for section 5
        When they click 'Continue'
        Then the error message shows 'Enter to what extent can you support that type of tuition'

    Scenario: Tuition responses show correct warning when there is no input for Tuition Plan
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner has arrived on respond to an enquiry page
        When the user has arrived on the tuition response page
        And they type '10' characters for section 1
        When they type '10' characters for section 2
        And they type '0' characters for section 3
        When they type '70' characters for section 4
        When they type '70' characters for section 5
        When they click 'Continue'
        Then the error message shows 'Enter can you support that tuition plan'

    Scenario: Send and Other Considerations are discluded as input options for TP
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner clicks a magic link with no info for optional inputs
        And the first response section is to be Key stage and subjects
        Then the key stages and subjects should match the request:
            | Section Name            | Expected Content                                                |
            | Key stages and subjects | Displays Key Stages and subjects table with the following data: |
            |                         | Key stage 1: English and Maths                                  |
            |                         | Key stage 2: English and Maths                                  |

        And the second response section is to be 'Tuition type:' with Type 'Any'
        And the third response section is to be "Tuition plan:" with text 'enquiry'
        Then the fourth response section is to be 'SEND requirements:' with text 'Not specified'
        And the Other considerations section is to be 'Other tuition considerations:' with text 'Not specified'
        When they type '70' characters for section 1
        And they type '80' characters for section 2
        And they type '90' characters for section 3
        Then they click 'Continue'
        Then the check your answers page does not include SEND and Other considerations