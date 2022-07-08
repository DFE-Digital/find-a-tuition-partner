Feature: User can view full details of a Tuition Parner
  Scenario: user directly accesses details page using the full name
    Given a user has arrived on the 'Tuition Partner' page for 'Action Tutoring'
    Then the page URL ends with '/action-tutoring'
    And the heading should say 'Action Tutoring'

  Scenario: user directly accesses details page using the SEO name
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    Then the page URL ends with '/bright-heart-education'
    And the heading should say 'Bright Heart Education'

