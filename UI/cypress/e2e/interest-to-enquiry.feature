Feature: School can respond to a response to an enquiry with an interested/rejected status

    Scenario: School can respond to an enquiry with an interested status
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        When the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        When a school navigates to the tp response page
        Then the school should see the tuition partner has responded with a default unread status

    Scenario: School can respond to an enquiry with an interested status
        Given a user has started the 'Find a tuition partner' journey
        Then a tuition partner clicks the magic link to respond to a schools enquiry
        And Tuition partner has journeyed to the check your answers page
        When the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        Then a school navigates to the tp response page
        And the school selects the first tuition partner and should see the tuition partners response
        And the school should have options to select 'I’m interested, contact tuition partner', 'No, I’m not interested' or 'I’m undecided'
        When the school clicks 'I’m interested, contact tuition partner'
        Then the status of the response should be updated to 'INTERESTED'

    Scenario: School can respond to an enquiry with a not interested status with first reason
        Given a user has started the 'Find a tuition partner' journey
        And a tuition partner clicks the magic link to respond to a schools enquiry
        And Tuition partner has journeyed to the check your answers page
        When the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        Then a school navigates to the tp response page
        And the school selects the first tuition partner and should see the tuition partners response
        And the school should have options to select 'I’m interested, contact tuition partner', 'No, I’m not interested' or 'I’m undecided'
        When the school has selected 'No, I’m not interested' for a response
        Then the school is navigated to the 'Are you sure' page
        Then the school clicks the 'Proceed and remove this tuition partner' button
        When the school clicks the radio button with value '1'
        And the school submits feedback
        Then there is text "You have removed 1 from your list of responses." visible

    Scenario: School can respond to an enquiry with a not interested status with invalid other reason
        Given a user has started the 'Find a tuition partner' journey
        And a tuition partner clicks the magic link to respond to a schools enquiry
        And Tuition partner has journeyed to the check your answers page
        When the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        Then a school navigates to the tp response page
        And the school selects the first tuition partner and should see the tuition partners response
        And the school should have options to select 'I’m interested, contact tuition partner', 'No, I’m not interested' or 'I’m undecided'
        When the school has selected 'No, I’m not interested' for a response
        Then the school is navigated to the 'Are you sure' page
        Then the school clicks the 'Proceed and remove this tuition partner' button
        When the school clicks the radio button with text 'other'
        And the school submits feedback
        Then they will see 'Enter your reasons for removing the tuition partner to submit your feedback' as an error message for the 'no other reason'

    Scenario: School can respond to an enquiry with a not interested status with valid other reason
        Given a user has started the 'Find a tuition partner' journey
        And a tuition partner clicks the magic link to respond to a schools enquiry
        And Tuition partner has journeyed to the check your answers page
        When the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        Then a school navigates to the tp response page
        And the school selects the first tuition partner and should see the tuition partners response
        And the school should have options to select 'I’m interested, contact tuition partner', 'No, I’m not interested' or 'I’m undecided'
        When the school has selected 'No, I’m not interested' for a response
        Then the school is navigated to the 'Are you sure' page
        Then the school clicks the 'Proceed and remove this tuition partner' button
        When the school clicks the radio button with text 'other'
        Then the additional information is added
        And the school submits feedback
        Then there is text "You have removed 3 from your list of responses." visible

    Scenario: School can respond to an enquiry with an I’m undecided status
        Given a user has started the 'Find a tuition partner' journey
        And a tuition partner clicks the magic link to respond to a schools enquiry
        And Tuition partner has journeyed to the check your answers page
        When the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        Then a school navigates to the tp response page
        And the school selects the first tuition partner and should see the tuition partners response
        And the school should have options to select 'I’m interested, contact tuition partner', 'No, I’m not interested' or 'I’m undecided'
        When the school clicks the 'I’m undecided' link
        Then the status of the response should be updated to 'UNDECIDED'

    Scenario: School's status is updated after clicking a TP response
        Given a user has started the 'Find a tuition partner' journey
        And a tuition partner clicks the magic link to respond to a schools enquiry
        And Tuition partner has journeyed to the check your answers page
        When the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        And a school navigates to the tp response page
        And the status of the response is 'UNREAD'
        And the school selects the first tuition partner and should see the tuition partners response
        And they click 'Back'
        Then the status of the response should be updated to 'UNDECIDED'

    Scenario: School skips feedback to a rejected enquiry
        Given a user has started the 'Find a tuition partner' journey
        And a tuition partner clicks the magic link to respond to a schools enquiry
        And Tuition partner has journeyed to the check your answers page
        When the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        Then a school navigates to the tp response page
        And the school selects the first tuition partner and should see the tuition partners response
        And the school should have options to select 'I’m interested, contact tuition partner', 'No, I’m not interested' or 'I’m undecided'
        When the school has selected 'No, I’m not interested' for a response
        Then the school is navigated to the 'Are you sure' page
        Then the school clicks the 'Proceed and remove this tuition partner' button
        When the school chooses to skip the feedback
        Then there is text "You have removed 4 from your list of responses." visible

    Scenario: Sorting columns by Tuition partner response updates the table to show TP in alphabetical order both ascending and descending
        Given a user has started the 'Find a tuition partner' journey
        And a school clicks the magic link to view their enquiry
        When the school selects to sort options by tuition partner response
        Then the tuition partners should be in ascending alphabetical order
        When the school selects to sort options by tuition partner response
        Then the tuition partners should be in descending alphabetical order

    Scenario: Sorting columns by Your interest to show TP's from responded to already read and vice versa
        Given a user has started the 'Find a tuition partner' journey
        And a school clicks the magic link to view their enquiry
        When the school selects to sort options by your interest
        Then the tuition partners who responded positively show first
        When the school selects to sort options by your interest
        Then the tuition partners who haven't read show first






