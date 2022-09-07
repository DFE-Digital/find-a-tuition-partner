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
    And the overlay search results filter heading is not displayed
  
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
  
  Scenario: Clicking show filters overlays filters in mobile phone view
    Given a user has arrived on the 'Search results' page
    And a user is using a 'phone'
    When they click 'Show filters'
    Then the search filters are displayed
  
  Scenario: Mobile filters overlay displays filters heading, results count and subjects header
    Given a mobile user has opened the mobile filters overlay
    Then the overlay search results filter heading is 'Filters'
    And the overlay search results filter heading has the results count
    And the overlay search results filter heading has the subjects header
  
  Scenario: Mobile filters overlay displays return to results link and show search results button
    Given a mobile user has opened the mobile filters overlay
    Then the return to results link is displayed
    And the show search results button is displayed

  Scenario: Selecting return to results link cancels filter changes
    Given a mobile user has opened the mobile filters overlay
    When they select subject 'Key stage 1 Maths'
    And they select the 'Return to results' link
    Then subject 'KeyStage1-Maths' is no longer selected
    And only the postcode and results sections are displayed

  Scenario: Selecting show search results button applies filter changes
    Given a mobile user has opened the mobile filters overlay
    When they select subject 'Key stage 1 Maths'
    And they select the 'Show search results' button
    Then subject 'KeyStage1-Maths' is selected
    And only the postcode and results sections are displayed