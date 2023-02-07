Feature: Tuition Partner cost information VAT
  Scenario: tuition cost information shows prices include VAT when tuition partner charges it
    Given a user has arrived on the 'Tuition Partner' page for 'FFT Education'
    Then the prices include VAT content is displayed

  Scenario: tuition cost information does not show prices include VAT when tuition partner does not charge it
    Given a user has arrived on the 'Tuition Partner' page for 'Coach Bright'
    Then the prices with VAT does not apply content is displayed for 'Coach Bright'
