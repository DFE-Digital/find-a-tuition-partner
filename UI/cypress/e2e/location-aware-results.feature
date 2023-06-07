Feature: Tuition Partner provides online but not face-to-face tuition in an area
  Scenario: Display list of TPs that offer services to that postcode
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    Then they will see the tuition partner 'Action Tutoring'

  Scenario: Only show tuition setting available in the postcode’s location on TP information page (location aware)
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    When they select the tuition partner 'Action Tutoring'
    Then they see the tuition settings 'Online'

  Scenario: Show all tuition settings on TP information page when the TP supports them all for the postcode’s location
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'S1 2PP'
    When they select the tuition partner 'Action Tutoring'
    Then they see the tuition settings 'Face-to-face, Online'

  Scenario: Only shows cost for tuition setting available in the postcode’s location
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    When they select the tuition partner 'Action Tutoring'
    Then they see the cost for tuition setting 'Online'

  Scenario: Show all cost for tuition setting when the TP supports them all for the postcode’s location
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'S1 2PP'
    When they select the tuition partner 'Action Tutoring'
    Then they see the cost for tuition setting 'Face-to-face, Online'