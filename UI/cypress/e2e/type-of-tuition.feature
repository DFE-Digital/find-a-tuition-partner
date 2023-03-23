Feature: Search By Type of Tuition Page

  Scenario: Clicking continue from subjects navigates to type of tuition
    Given a user has arrived on the 'Which subjects' page for 'Key stage 3, Key stage 4'
    And they select subjects for the key stages
    When they click 'Continue'
    And they will be taken to the type of tuition page
    Then the correct options will display


  Scenario: subjects page title is 'What type of tuition do you need?'
    Given the 'Type of tuition' page is displayed
    Then the page's title is 'What type of tuition do you need?'

  Scenario: Back click returns to subjects for key stages input page
    Given the 'Type of tuition' page is displayed
    When they click 'Back'
    Then a user has arrived on the 'Which subjects' page for 'Key stage 3, Key stage 4'
    And they are shown the subjects for 'Key stage 3, Key stage 4'

  Scenario: User must select a type of tuition
    Given the 'Type of tuition' page is displayed
    When they click 'Continue'
    Then they will see 'Select a type of tuition' as an error message for the 'tuition type'


  Scenario: search by type of tuition page url is '/which-tuition-types'
    Given the 'Type of tuition' page is displayed
    Then they will be taken to the 'What type of tuition do you need?' page


  Scenario: the user is only able to select on of the three options for type of tuition
    Given the 'Type of tuition' page is displayed
    And the correct options will display
    When user clicks the button with text 'Any'
    Then the 'Any' button is selected and the other two buttons are not
    When user clicks the button with text 'Online'
    Then the 'Online' button is selected and the other two buttons are not
    When user clicks the button with text 'In School'
    Then the 'In School' button is selected and the other two buttons are not


  Scenario: the user clicks continue and the page redirects to search results page
    Given the 'Type of tuition' page is displayed
    When user clicks the button with text 'In School'
    And they click 'Continue'
    Then they will be taken to the 'Search Results' page
    And the filter results show the expected selection

  Scenario: user clicks service name on key stages page
    Given a user has arrived on the 'Which key stages' page
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page