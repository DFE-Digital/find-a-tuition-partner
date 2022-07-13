Feature: User can choose Key Stage and Subject 
  Scenario: All key stages are shown
    Given a user has arrived on the 'Which key stages' page
    Then they will see all the keys stages as options

  Scenario: Back click returns to location input page
    Given a user has arrived on the 'Which key stages' page for postcode 'AB1 2CD'
    When they click 'Back'
    Then they will be taken to the 'Find a tuition partner' journey start page
    And they will see 'AB1 2CD' entered for the postcode

  Scenario: Key stage must be selected to move to the next page
    Given a user has arrived on the 'Which key stages' page
    When they click 'Continue'
    Then they will see 'Select at least one key stage' as an error message for the 'keystages'

  Scenario Outline: Click continue with any combination of key stages selected
    Given a user has arrived on the 'Which key stages' page
    When they select '<keyStages>'
    And they click 'Continue'
    Then they are shown the subjects for '<keyStages>'
    Examples:
    | keyStages |
    | Key stage 1 |
    | Key stage 1, Key stage 2 |
    | Key stage 1, Key stage 3 |
    | Key stage 3 |
    | Key stage 3, Key stage 4 |
    | Key stage 1, Key stage 2, Key stage 3, Key stage 4 |

  Scenario: User lands on subjects page without associated key stages selected
    Given a user has started the 'Find a tuition partner' journey
    When they manually navigate to the 'Which subjects' page 
    Then they are shown all the subjects under all the keys stages

  Scenario: User must select a subject
    Given a user has arrived on the 'Which subjects' page for Key stage 1
    When they click 'Continue'
    Then they will see 'Select the subject or subjects' as an error message for the 'subjects'
    And they are shown the subjects for 'Key stage 1'

  Scenario: Back click returns to key stages input page
    Given a user has arrived on the 'Which subjects' page for Key stage 1
    When they click 'Back'
    Then they will be taken to the 'Which key stages' page
    And they will see 'Key stage 1' selected
