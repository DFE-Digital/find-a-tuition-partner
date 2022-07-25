Feature: Display SEND information
  Scenario: Show tuition partners' SEND status on the search results page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 Maths'
    Then the SEND status is '<SEND>' for tuition partner '<partner>'
    Examples:
    | SEND | partner                |
    | No   | Action Tutoring        |
    | Yes  | Bright Heart Education |
