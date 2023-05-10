Feature: Phase banner shows users service is still being worked on and feedback can help improve it
  Scenario: phase banner state is 'beta'
    Given a user has started the 'Find a tuition partner' journey
    Then they will see the phase banner
    And the current phase is 'beta'

  Scenario: phase banner contains link to feedback form
    Given a user has started the 'Find a tuition partner' journey
    Then the phase banner feedback link 'feedback' links to the feedback page

