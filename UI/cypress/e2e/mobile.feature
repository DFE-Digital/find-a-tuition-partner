Feature: Mobile view page tests
  
    Scenario: Subject list is bullet pointed on search reaults page in mobile view
        Given a user has started the 'Find a tuition partner' journey
            And a user is using a 'phone' 
        When the search result page is displayed
        Then the subject list is bullet pointed

    Scenario: Subject list is bullet pointed on tuition partner page in mobile view
       Given a user has started the 'Find a tuition partner' journey
            And a user is using a 'phone' 
        When a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
        Then the subject list is bullet pointed

    Scenario: Subject list is not bullet pointed on search reaults page in non mobile view
        Given a user has started the 'Find a tuition partner' journey 
        When the search result page is displayed
        Then the subject list is not bullet pointed

    Scenario: Subject list is not bullet pointed on tuition partner page in non mobile view
       Given a user has started the 'Find a tuition partner' journey
        When a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
        Then the subject list is not bullet pointed