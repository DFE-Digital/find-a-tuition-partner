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
        Then the search results page heading is 'Find a tuition partner'
    
    Scenario: Search results page heading is 'Search results' in mobile phone view
        Given a user has arrived on the 'Search results' page
        And a user is using a 'phone'
        Then the search results page heading is 'Search results'
    
    Scenario: Search results filter heading is 'Filter results' in tablet and above view
        Given a user has arrived on the 'Search results' page
        And a user is using a 'tablet'
        Then the search results filter heading is displayed
        And the search results filter heading is 'Filter results'
        And the overlay search results filter heading is be displayed
    
    Scenario: Search results page shows filters and results in tablet and above view
        Given a user has arrived on the 'Search results' page
        And a user is using a 'tablet'
        Then the search filters, postcode and results sections are all displayed
    
    Scenario: Search results page does not show filters in mobile phone view
        Given a user has arrived on the 'Search results' page
        And a user is using a 'phone'
        Then only the postcode and results sections are displayed
    
    Scenario: Search results page does not have show filters button in tablet and above view
        Given a user has arrived on the 'Search results' page
        And a user is using a 'tablet'
        Then the show filters button is not displayed
    
    Scenario: Search results page does has show filters button in mobile phone view
        Given a user has arrived on the 'Search results' page
        And a user is using a 'phone'
        Then the show filters button is displayed