Feature: The Check Your Answers page for the Enquiry Builder

    Scenario: Navigate to the Check Your Answers page
        Given a user has started the 'Find a tuition partner' journey
        When user has journeyed forward to the check your answers page
        Then they are redirected to the check your answers page

    Scenario: Verify Check Your Answers page content
        Given a user has started the 'Find a tuition partner' journey
        When user has journeyed forward to the check your answers page
        Then they are redirected to the check your answers page
        Then the Check Your Answers page displays the following:
            | Section Name            | Expected Content                                |
            | Key stages and subjects | Displays Key Stages and subjects table          |
            | Tuition setting         | No preference                                   |
            | Tuition Plan            | enquiry                                         |
            | SEND Support            | enquiry                                         |
            | Other Considerations    | enquiry                                         |
            | Your school details     | Oxford Spires Academy, OX4 2AU                  |
            | Email Address           | simulate-delivered@notifications.service.gov.uk |

    Scenario: Change selections for Key Stages and Subjects
        Given a user has started the 'Find a tuition partner' journey
        And user has journeyed forward to the check your answers page
        When the user clicks the change button '1'
        Then the user is taken back to the key-stage page to change their selection
        And they will see 'Key stage 1, Key stage 2' selected
        When they select Key stage 3
        And they click 'Continue'
        Then the user is taken to the subjects page to change their selection
        When they select science for all three key stages
        And they click 'Continue'
        When they are redirected to the check your answers page
        Then the Check Your Answers page displays the following with the key stage and subjects updates:
            | Section Name            | Expected Content                                |
            | Key stages and subjects | Displays Key Stage and Subjects table           |
            | Tuition setting         | No preference                                   |
            | Email Address           | simulate-delivered@notifications.service.gov.uk |
            | Tuition Plan            | enquiry                                         |
            | SEND Support            | enquiry                                         |
            | Other Considerations    | enquiry                                         |

    Scenario: Change selections for tuition setting
        Given a user has started the 'Find a tuition partner' journey
        And user has journeyed forward to the check your answers page
        When the user clicks the change button '2'
        Then the user is taken back to the tuition setting page
        And they select 'Online'
        And they click 'Continue'
        Then they are redirected to the check your answers page
        Then the Check Your Answers page displays the following with the tuition setting update:
            | Section Name            | Expected Content                                |
            | Key stages and subjects | Displays Key Stage and Subjects table           |
            | Tuition setting         | Online                                          |
            | Email Address           | simulate-delivered@notifications.service.gov.uk |
            | Tuition Plan            | enquiry                                         |
            | SEND Support            | enquiry                                         |
            | Other Considerations    | enquiry                                         |

    Scenario: Change selections for email address
        Given a user has started the 'Find a tuition partner' journey
        And user has journeyed forward to the check your answers page
        When the user clicks the change button for email address
        Then the user is taken back to the email address page
        And they enter another email address
        And they click 'Continue'
        Then they are redirected to the check your answers page
        Then the Check Your Answers page displays the following with the email address update:
            | Section Name            | Expected Content                                |
            | Key stages and subjects | Displays Key Stage and Subjects table           |
            | Tuition setting         | No preference                                   |
            | Email Address           | simulate-delivered@notifications.service.gov.uk |
            | Tuition Plan            | enquiry                                         |
            | SEND Support            | enquiry                                         |
            | Other Considerations    | enquiry                                         |

    Scenario: Change selections for tuition questions
        Given a user has started the 'Find a tuition partner' journey
        And user has journeyed forward to the check your answers page
        When the user clicks the change button '3'
        Then they are redirected to the enquiry question page
        When they type '5' characters for question 1
        And they click 'Continue'
        Then they are redirected to the check your answers page
        When the user clicks the change button '4'
        Then they are redirected to the SEND requirements page
        When they type '5' characters for question 2
        And they click 'Continue'
        Then they are redirected to the check your answers page
        When the user clicks the change button '5'
        Then they are redirected to the other requirements page
        When they type '0' characters for question 3
        And they click 'Continue'
        Then they are redirected to the check your answers page
        Then the Check Your Answers page displays the following with the email address update:
            | Section Name            | Expected Content                                |
            | Key stages and subjects | Displays Key Stage and Subjects table           |
            | Tuition setting         | No preference                                   |
            | Email Address           | simulate-delivered@notifications.service.gov.uk |
            | Tuition Plan            | aaaaa                                           |
            | SEND Support            | aaaaa                                           |
            | Other Considerations    | Not specified                                   |



    Scenario: Clear search filters in search page and attempt to make an enquiry
        Given user navigates to check your answers unselecting filter results
        And they select terms and conditions
        Then they click send enquiry
        Then the correct error message should display for no keystage and subjects selected

    Scenario: Clear search filters in search page and changing to correct input to make an enquiry
        Given user navigates to check your answers unselecting filter results
        And they select terms and conditions
        Then they click send enquiry
        Then the correct error message should display for no keystage and subjects selected
        When the user clicks the change button '1'
        When they select 'Key stage 3'
        And they click 'Continue'
        When they select 'Key stage 3 Science'
        And they click 'Continue'
        When they are redirected to the check your answers page
        Then the key stages and subjects show 'Key stage 3: Science'

    Scenario: user clicks service name on check your answers page
        Given a user has started the 'Find a tuition partner' journey
        When user has journeyed forward to the check your answers page
        When they click the 'Find a tuition partner' service name link
        Then they will be taken to the 'Find a tuition partner' journey start page
        When they click back on the browser
        Then they are redirected to the check your answers page


    Scenario: Submitting without selecting the checkbox should cause an error
        Given a user has started the 'Find a tuition partner' journey
        And user has journeyed forward to the check your answers page
        When they click send enquiry
        Then they will see the correct error for not checking the t and c

    Scenario: Clearing user session clears data saved session timeout page is shown
        Given a user has started the 'Find a tuition partner' journey
        When user has journeyed forward to the check your answers page
        And the user clears the current sessions data
        And they click 'Back'
        Then the session timeout page is shown