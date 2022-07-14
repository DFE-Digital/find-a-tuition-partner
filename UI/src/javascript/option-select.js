window.GOVUK = window.GOVUK || {}
window.GOVUK.Modules = window.GOVUK.Modules || {};

(function (Modules) {
  /* This JavaScript provides two functional enhancements to option-select components:
    1) A count that shows how many results have been checked in the option-container
    2) Open/closing of the list of checkboxes
  */
  function OptionSelect ($module) {
    this.$optionSelect = $module
    this.$options = this.$optionSelect.querySelectorAll("input[type='checkbox']")
    this.$optionsContainer = this.$optionSelect.querySelector('.js-options-container')
    this.$optionList = this.$optionsContainer.querySelector('.js-auto-height-inner')
    this.$allCheckboxes = this.$optionsContainer.querySelectorAll('.govuk-checkboxes__item')

    this.checkedCheckboxes = []
  }

  OptionSelect.prototype.init = function () {
    // Replace div.container-head with a button
    this.replaceHeadingSpanWithButton()

    // Add js-collapsible class to parent for CSS
    this.$optionSelect.classList.add('js-collapsible')

    // Add open/close listeners
    var button = this.$optionSelect.querySelector('.js-container-button')
    button.addEventListener('click', this.toggleOptionSelect.bind(this))

    var closedOnLoad = this.$optionSelect.getAttribute('data-closed-on-load')
    if (closedOnLoad === 'true') {
      this.close()
    }
  }

  OptionSelect.prototype.cleanString = function cleanString (text) {
    text = text.replace(/&/g, 'and')
    text = text.replace(/[’',:–-]/g, '') // remove punctuation characters
    text = text.replace(/[.*+?^${}()|[\]\\]/g, '\\$&') // escape special characters
    return text.trim().replace(/\s\s+/g, ' ').toLowerCase() // replace multiple spaces with one
  }

  OptionSelect.prototype.getAllCheckedCheckboxes = function getAllCheckedCheckboxes () {
    this.checkedCheckboxes = []

    for (var i = 0; i < this.$options.length; i++) {
      if (this.$options[i].checked) {
        this.checkedCheckboxes.push(i)
      }
    }
  }

  OptionSelect.prototype.replaceHeadingSpanWithButton = function replaceHeadingSpanWithButton () {
    /* Replace the span within the heading with a button element. This is based on feedback from Léonie Watson.
     * The button has all of the accessibility hooks that are used by screen readers and etc.
     * We do this in the JavaScript because if the JavaScript is not active then the button shouldn't
     * be there as there is no JS to handle the click event.
    */
    var containerHead = this.$optionSelect.querySelector('.js-container-button')
    var jsContainerHeadHTML = containerHead.innerHTML

    // Create button and replace the preexisting html with the button.
    var button = document.createElement('button')
    button.setAttribute('class', 'js-container-button app-c-option-select__title app-c-option-select__button')
    // Add type button to override default type submit when this component is used within a form
    button.setAttribute('type', 'button')
    button.setAttribute('aria-expanded', true)
    button.setAttribute('id', containerHead.getAttribute('id'))
    button.setAttribute('aria-controls', this.$optionsContainer.getAttribute('id'))
    button.innerHTML = jsContainerHeadHTML
    containerHead.parentNode.replaceChild(button, containerHead)
  }

  OptionSelect.prototype.toggleOptionSelect = function toggleOptionSelect (e) {
    if (this.isClosed()) {
      this.open()
    } else {
      this.close()
    }
    e.preventDefault()
  }

  OptionSelect.prototype.open = function open () {
    if (this.isClosed()) {
      this.$optionSelect.querySelector('.js-container-button').setAttribute('aria-expanded', true)
      this.$optionSelect.classList.remove('js-closed')
      this.$optionSelect.classList.add('js-opened')
    }
  }

  OptionSelect.prototype.close = function close () {
    this.$optionSelect.classList.remove('js-opened')
    this.$optionSelect.classList.add('js-closed')
    this.$optionSelect.querySelector('.js-container-button').setAttribute('aria-expanded', false)
  }

  OptionSelect.prototype.isClosed = function isClosed () {
    return this.$optionSelect.classList.contains('js-closed')
  }

  OptionSelect.prototype.setContainerHeight = function setContainerHeight (height) {
    this.$optionsContainer.style.height = height + 'px'
  }

  OptionSelect.prototype.isCheckboxVisible = function isCheckboxVisible (option) {
    var initialOptionContainerHeight = this.$optionsContainer.clientHeight
    var optionListOffsetTop = this.$optionList.getBoundingClientRect().top
    var distanceFromTopOfContainer = option.getBoundingClientRect().top - optionListOffsetTop
    return distanceFromTopOfContainer < initialOptionContainerHeight
  }

  OptionSelect.prototype.getVisibleCheckboxes = function getVisibleCheckboxes () {
    var visibleCheckboxes = []
    for (var i = 0; i < this.$options.length; i++) {
      if (this.isCheckboxVisible(this.$options[i])) {
        visibleCheckboxes.push(this.$options[i])
      }
    }

    // add an extra checkbox, if the label of the first is too long it collapses onto itself
    if (this.$options[visibleCheckboxes.length]) {
      visibleCheckboxes.push(this.$options[visibleCheckboxes.length])
    }
    return visibleCheckboxes
  }

  Modules.OptionSelect = OptionSelect
})(window.GOVUK.Modules)