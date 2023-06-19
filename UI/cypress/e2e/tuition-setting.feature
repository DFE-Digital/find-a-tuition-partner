Feature: Search By Tuition setting Page

  Scenario: Clicking continue from subjects navigates to tuition setting
    Given a user has arrived on the 'Which subjects' page for 'Key stage 3, Key stage 4'
    And they select subjects for the key stages
    When they click 'Continue'
    And they will be taken to the tuition setting page
    Then the correct options will display


  Scenario: subjects page title is 'What tuition setting do you prefer?'
    Given the 'Tuition setting' page is displayed
    Then the page's title is 'What tuition setting do you prefer?'

  Scenario: Back click returns to subjects for key stages input page
    Given the 'Tuition setting' page is displayed
    When they click 'Back'
    Then a user has arrived on the 'Which subjects' page for 'Key stage 3, Key stage 4'
    And they are shown the subjects for 'Key stage 3, Key stage 4'

  Scenario: User must select a tuition setting
    Given the 'Tuition setting' page is displayed
    When they click 'Continue'
    Then they will see 'Select a tuition setting' as an error message for the 'tuition setting'


  Scenario: search by tuition setting page url is '/which-tuition-settings'
    Given the 'Tuition setting' page is displayed
    Then they will be taken to the 'What tuition setting do you prefer?' page


  Scenario: the user is only able to select on of the three options for tuition setting
    Given the 'Tuition setting' page is displayed
    And the correct options will display
    When user clicks the button with text 'No preference'
    Then the 'No preference' button is selected and the other two buttons are not
    When user clicks the button with text 'Online'
    Then the 'Online' button is selected and the other two buttons are not
    When user clicks the button with text 'Face-to-face'
    Then the 'Face-to-face' button is selected and the other two buttons are not


  Scenario: the user clicks continue and the page redirects to search results page
    Given the 'Tuition setting' page is displayed
    When user clicks the button with text 'Face-to-face'
    And they click 'Continue'
    Then they will be taken to the 'Search Results' page
    And the filter results show the expected selection

  Scenario: user clicks service name on key stages page
    Given a user has arrived on the 'Which key stages' page
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page