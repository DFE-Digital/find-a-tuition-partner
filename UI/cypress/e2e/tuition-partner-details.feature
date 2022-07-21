Feature: User can view full details of a Tuition Parner
  Scenario: page title is 'Name of Tuition Partner'
    Given a user has arrived on the 'Tuition Partner' page for 'Connex Education Partnership'
    Then the page's title is 'Connex Education Partnership'

  Scenario: user clicks service name
    Given a user has started the 'Find a tuition partner' journey
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: user directly accesses details page using the full name
    Given a user has arrived on the 'Tuition Partner' page for 'Action Tutoring'
    Then the page URL ends with '/action-tutoring'
    And the heading should say 'Action Tutoring'

  Scenario: user directly accesses details page using the SEO name
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    Then the page URL ends with '/bright-heart-education'
    And the heading should say 'Bright Heart Education'

  Scenario: don’t show empty fields where TP has not provided information
    Given a user has arrived on the 'Tuition Partner' page for 'connex-education-partnership'
    Then TP has not provided the information in the 'Address' section

  Scenario: don’t show empty fields where TP has not provided information
    Given a user has arrived on the 'Tuition Partner' page for 'lancashire-county-council'
    Then TP has not provided the information in the 'Phone Number' section

  Scenario: show Contact Details where TP has provided information
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    Then TP has provided full contact details

  Scenario: home page is selected 
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    When the home page is selected
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: back is selected return to search results page
    Given a user has arrived on the 'Tuition Partner' page for 'Bright Heart Education' after entering search details
    When they click 'Back'
    Then the page URL ends with '/search-results'
    And the search details are correct

  Scenario: quality assured tuition partner and payment details is initially hidden
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    Then the quality assured tuition partner details are hidden
    And the payment details are hidden

  Scenario: user clicks quality assured tuition partner details summary
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    When they click 'What is a quality assured tuition partner?'
    Then the quality assured tuition partner details are shown

  Scenario: user has access to website link of tution partner
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    Then the tuition partners website link exist
  
  Scenario: user has access to funding guidance page
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    Then the funding guidance page is accessible
  
  
  