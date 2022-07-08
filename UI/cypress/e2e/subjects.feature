Feature: User can choose Key Stage and Subject 
  Scenario: User lands on subjects page without associated key stages selected
    Given a user has started the 'Find a tuition partner' journey
    When they manually navigate to the 'Which subjects' page 
    Then they are shown all the subjects under all the keys stages

  Scenario: User lands on subjects page without associated key stages selected
    Given a user has arrived on the 'Which subjects' page for Key stage 1
    When they click 'Continue'
    Then they will see 'Select the subject or subjects' as an error message for the 'subjects'
    And they are shown the subjects for Key stage 1

  Scenario: Back click returns to key subjects input page
    Given a user has arrived on the 'Which subjects' page for Key stage 1
    When they click 'Back'
    Then they will be taken to the 'Which key stages' page
    And they will see 'Key stage 1' selected

  Scenario: Back click returns to location input page
    Given a user has arrived on the 'Which key stages' page for postcode 'AB1 2CD'
    When they click 'Back'
    Then they will be taken to the 'Find a tuition partner' journey start page
    And they will see 'AB1 2CD' entered for the postcode

