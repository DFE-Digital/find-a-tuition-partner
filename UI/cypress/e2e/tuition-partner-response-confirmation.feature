Feature: Confirmation page for tuition partner response

    Scenario: Submitting a successful response shows the confirmation page correctly
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        When  the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        Then the user has arrived on the tuition response confirmation page
        And the page has title 'Response sent'
        And a unique reference number is shown

    Scenario: User attempts to go back after confirmation
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        When  the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        Then the user has arrived on the tuition response confirmation page
        When they click back on browser
        Then the session timeout page is shown