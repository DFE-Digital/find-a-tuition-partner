Feature: Tuition partner details mobile view page tests
  
    Scenario: Subject list is bullet pointed on search results page in mobile phone view
        Given a user has arrived on the 'Search results' page
        And a user is using a 'phone' 
        Then the subject list is bullet pointed

    Scenario: Subject list is bullet pointed on tuition partner page in mobile phone view
        Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
        And a user is using a 'phone' 
        Then the subject list is bullet pointed

    Scenario: Subject list is not bullet pointed on search results page in tablet and above view
        Given a user has arrived on the 'Search results' page
        And a user is using a 'tablet' 
        Then the subject list is not bullet pointed

    Scenario: Subject list is not bullet pointed on tuition partner page in tablet and above view
        Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
        And a user is using a 'tablet'
        Then the subject list is not bullet pointed
    
    Scenario: Search results page heading is 'Find a tuition partner' in tablet and above view
        Given a user has arrived on the 'Search results' page
        And a user is using a 'tablet'
        Then the search results page heading should be 'Find a tuition partner'
    
    Scenario: Search results page heading is 'Search results' in mobile phone view
        Given a user has arrived on the 'Search results' page
        And a user is using a 'phone'
        Then the search results page heading should be 'Search results'