Feature: Tuition Partner provides online but not in-school tuition in an area
  Scenario: Display list of TPs that offer services to that postcode
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    When they select 'any' tuition type
    Then they will see the tuition partner 'Action Tutoring'

  Scenario: Only show tuition type available in the postcode’s location on TP information page (location aware)
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    When they select 'any' tuition type
    And they select the tuition partner 'Action Tutoring'
    Then they see only tuition types available for postcode 'SK1 1EB'