Feature: Show all the links at the footer

    Scenario: Footer should have 'five' page links
        Given a user has arrived on the 'find-a-tuition-partner' website
        Then they should see '5' page links at the footer

    Scenario: User should see Contact us link at the footer as first link
        Given a user has arrived on the 'find-a-tuition-partner' website
        Then the 'first' link they see is 'Contact us' with test id 'govuk-footer-link'
        And they will see link to '/contact-us?FromReturnUrl=/' with test id 'contact-us-link'

    Scenario: User should see Accessibility link at the footer as second link
        Given a user has arrived on the 'find-a-tuition-partner' website
        Then the 'second' link they see is 'Accessibility' with test id 'govuk-footer-link'
        And they will see link to '/accessibility?FromReturnUrl=/' with test id 'accessibility-link'

    Scenario: User should see Cookies link at the footer as third link
        Given a user has arrived on the 'find-a-tuition-partner' website
        Then the 'third' link they see is 'Cookies' with test id 'govuk-footer-link'
        And they will see link to '/cookies?FromReturnUrl=/' with test id 'view-footer-cookies'

    Scenario: User should see Privacy link at the footer as fourth link
        Given a user has arrived on the 'find-a-tuition-partner' website
        Then the 'fourth' link they see is 'Privacy' with test id 'govuk-footer-link'
        And they will see link to '/privacy?FromReturnUrl=/' with test id 'privacy-link'

    Scenario: User should see Report issues at the footer as fifth link
        Given a user has arrived on the 'find-a-tuition-partner' website
        Then the 'fifth' link they see is 'Report issues' with test id 'govuk-footer-link'
        And they will see link to '/report-issues?FromReturnUrl=/' with test id 'report-issues-link'