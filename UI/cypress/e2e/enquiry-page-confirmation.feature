Feature: Enquiry Confirmation Page

    Scenario: User submits successful enquiry
        Given a user has started the 'Find a tuition partner' journey
        When user has journeyed forward to the check your answers page
        And they select terms and conditions
        And they click send enquiry
        Then the confirmation page is shown
        And the page has title Request sent
        And a unique reference number is shown

    Scenario: User attempts to go back after confirmation
        Given a user has started the 'Find a tuition partner' journey
        When user has journeyed forward to the check your answers page
        And they select terms and conditions
        And they click send enquiry
        Then the confirmation page is shown
        When they click back on browser
        Then the session timeout page is shown

    Scenario: User has different reference numbers for new enquires
        Given a user has started the 'Find a tuition partner' journey
        When user has journeyed forward to the check your answers page
        And they select terms and conditions
        And they click send enquiry
        Then the confirmation page is shown
        When user creates another enquiry
        And they select terms and conditions
        And they click send enquiry
        Then the second reference number is dfferent to the first
