import { ref, watch, nextTick } from 'vue';
import { useSpeechRecognition } from '@vueuse/core';

/**
 * Composable for managing speech recognition in text input fields
 * @param {Object} options - Configuration options
 * @param {string} options.inputId - ID of the text input element
 * @param {string} options.lang - Language for speech recognition (default: 'en-US')
 * @param {boolean} options.continuous - Whether recognition should be continuous (default: true)
 * @param {boolean} options.interimResults - Whether to show interim results (default: true)
 * @param {Function} options.onTextChange - Callback when text changes
 * @param {number} options.autoStopTimeout - Auto-stop timeout in milliseconds (default: 1500)
 */
export function useSpeechInput(options = {}) {
  const {
    inputId = 'text-input-elem',
    lang = 'en-US',
    continuous = true,
    interimResults = true,
    onTextChange = null,
    autoStopTimeout = 1500,
  } = options;

  const hasText = ref(false);
  const microphoneError = ref(null);
  const autoStopTimer = ref(null);
  const maxListeningTimer = ref(null);
  const microphoneErrorTimer = ref(null); // Timer for auto-clearing error messages
  const clickOutsideHandler = ref(null);
  const resultProcessed = ref(false); // Flag to track if result was processed
  const shouldTriggerSearch = ref(false); // Flag to trigger search
  const successfulResult = ref(false); // Global flag to prevent any error display

  /**
   * Cleans speech recognition result text
   * @param {string} text - The raw speech recognition text
   * @returns {string} - Cleaned text
   */
  const cleanSpeechText = (text) => {
    if (!text) return '';
    
    let cleaned = text.trim();
    
    // Remove common punctuation that might interfere with search
    cleaned = cleaned.replace(/[.!?,:;]$/g, '');
    
    return cleaned;
  };

  const { 
    isListening, 
    result, 
    start, 
    stop, 
    isSupported 
  } = useSpeechRecognition({
    continuous,
    interimResults,
    lang,
  });

  /**
   * Sets an error message and automatically clears it after 3 seconds
   * @param {string} errorMessage - The error message to display
   */
  const setMicrophoneError = (errorMessage) => {
    // Clear any existing error timer
    clearMicrophoneErrorTimer();
    
    // Set the error message
    microphoneError.value = errorMessage;
    
    // Set timer to clear the error after 3 seconds
    microphoneErrorTimer.value = setTimeout(() => {
      microphoneError.value = null;
      microphoneErrorTimer.value = null;
    }, 3000);
  };

  /**
   * Clears the microphone error timer
   */
  const clearMicrophoneErrorTimer = () => {
    if (microphoneErrorTimer.value) {
      clearTimeout(microphoneErrorTimer.value);
      microphoneErrorTimer.value = null;
    }
  };

  /**
   * Toggles speech recognition state
   */
  const toggleSpeechRecognition = async () => {
    if (!isSupported.value) {
      setMicrophoneError('Speech recognition not supported in this browser');
      return;
    }

    if (isListening.value) {
      stopSpeechRecognition();
    } else {
      try {
        // Clear input and reset state before starting
        clearAndReset();
        
        // Reset ALL flags
        resultProcessed.value = false;
        successfulResult.value = false;
        
        // Test microphone access
        const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
        stream.getTracks().forEach(track => track.stop());
        
        microphoneError.value = null;
        clearMicrophoneErrorTimer();
        
        // Start fresh recognition - Let the browser handle timing naturally
        start();
        addClickOutsideListener();
        
        // Only add a simple fallback timer for real timeout cases (much longer)
        startMaxListeningTimer();
      } catch (e) {
        if (e.name === 'NotAllowedError' || e.name === 'PermissionDeniedError' || 
            e.message.includes('denied') || e.message.includes('Permission denied')) {
          // Permission error - show modal with auto-clear
          setMicrophoneError('Access to the microphone was denied. Please check your browser settings.');
        } else if (e.name === 'NotFoundError' || e.name === 'DevicesNotFoundError' ||
                   e.message.includes('not found') || e.message.includes('No device found')) {
          // Device not found error - show tooltip with auto-clear
          setMicrophoneError('No microphone was detected. Please connect one and try again.');
        } else {
          // Generic error - assume device problem by default with auto-clear
          setMicrophoneError('No microphone was detected. Please connect one and try again.');
        }
      }
    }
  };

  /**
   * Stops speech recognition and cleans up all timers and listeners
   */
  const stopSpeechRecognition = () => {
    stop();
    clearAutoStopTimer();
    clearMaxListeningTimer();
    removeClickOutsideListener();
  };

  /**
   * Starts the auto-stop timer
   */
  const startAutoStopTimer = () => {
    clearAutoStopTimer();
    autoStopTimer.value = setTimeout(() => {
      if (isListening.value && !successfulResult.value) {
        stopSpeechRecognition();
        // Only show error if no result was processed and no successful result flag
        if (!resultProcessed.value && !successfulResult.value) {
          setMicrophoneError("No speech detected. Please try again.");
        }
      }
    }, autoStopTimeout);
  };

  /**
   * Clears the auto-stop timer
   */
  const clearAutoStopTimer = () => {
    if (autoStopTimer.value) {
      clearTimeout(autoStopTimer.value);
      autoStopTimer.value = null;
    }
  };

  /**
   * Starts the maximum listening timer (3 seconds) - Only for real timeouts
   */
  const startMaxListeningTimer = () => {
    clearMaxListeningTimer();
    maxListeningTimer.value = setTimeout(() => {
      if (isListening.value && !successfulResult.value) {
        stopSpeechRecognition();
        // Only show error if no result was processed and no successful result flag
        if (!resultProcessed.value && !successfulResult.value) {
          setMicrophoneError("We couldn't recognize your voice input. Please try again.");
        }
      }
    }, 3000);
  };

  /**
   * Clears the maximum listening timer
   */
  const clearMaxListeningTimer = () => {
    if (maxListeningTimer.value) {
      clearTimeout(maxListeningTimer.value);
      maxListeningTimer.value = null;
    }
  };

  /**
   * Adds click outside listener to cancel speech recognition
   */
  const addClickOutsideListener = () => {
    removeClickOutsideListener();
    
    clickOutsideHandler.value = (event) => {
      // Check if click is on the backdrop (blur background)
      if (event.target.classList.contains('speech-listening-backdrop')) {
        if (isListening.value) {
          stopSpeechRecognition();
        }
        return;
      }
      
      const microphoneButton = document.querySelector('[data-microphone-button]');
      const speechModal = document.querySelector('.speech-listening-modal');
      const speechOverlay = document.querySelector('.speech-listening-overlay');
      const searchContainer = document.getElementById(inputId)?.closest('#search-input-with-icon-div');
      
      // Check if click is outside the microphone button, modal, and search container
      const isOutsideMicButton = microphoneButton && !microphoneButton.contains(event.target);
      const isOutsideModal = speechModal && !speechModal.contains(event.target);
      const isOutsideOverlay = speechOverlay && !speechOverlay.contains(event.target);
      const isOutsideSearchContainer = searchContainer && !searchContainer.contains(event.target);
      
      // Cancel if click is outside all relevant elements
      if (isOutsideMicButton && isOutsideModal && isOutsideOverlay && isOutsideSearchContainer) {
        if (isListening.value) {
          stopSpeechRecognition();
        }
      }
    };
    
    document.addEventListener('click', clickOutsideHandler.value, true);
  };

  /**
   * Removes click outside listener
   */
  const removeClickOutsideListener = () => {
    if (clickOutsideHandler.value) {
      document.removeEventListener('click', clickOutsideHandler.value, true);
      clickOutsideHandler.value = null;
    }
  };

  /**
   * Handles speech recognition result
   */
  const handleSpeechResult = () => {
    if (result.value && result.value.trim().length > 0) {
      // IMMEDIATELY set success flag to prevent ANY error display
      successfulResult.value = true;
      
      // IMMEDIATELY cancel ALL timers and clear errors
      clearAutoStopTimer();
      clearMaxListeningTimer();
      microphoneError.value = null;
      clearMicrophoneErrorTimer();
      resultProcessed.value = true;
      
      const inputEl = document.getElementById(inputId);
      if (inputEl) {
        const cleanedText = cleanSpeechText(result.value);
        
        if (cleanedText.length > 0) {
          // Apply text immediately
          inputEl.value = cleanedText;
          hasText.value = true;
          
          // Trigger input event and callback immediately
          inputEl.dispatchEvent(new Event('input', { bubbles: true }));
          if (onTextChange && typeof onTextChange === 'function') {
            onTextChange(cleanedText);
          }
          
          // Stop listening immediately after getting valid result
          if (isListening.value) {
            stopSpeechRecognition();
          }
          
          // Trigger search after a short delay to ensure DOM is updated
          setTimeout(() => {
            shouldTriggerSearch.value = true;
          }, 50);
        }
      }
    }
  };

  /**
   * Cleanup function to remove all listeners and timers
   */
  const cleanup = () => {
    clearAutoStopTimer();
    clearMaxListeningTimer();
    clearMicrophoneErrorTimer();
    removeClickOutsideListener();
  };

  /**
   * Updates hasText state based on input
   */
  const updateTextState = () => {
    const inputEl = document.getElementById(inputId);
    if (inputEl) {
      hasText.value = inputEl.value.length > 0;
    }
  };

  /**
   * Clears the input content
   * @param {Event} event - Keyboard or click event
   */
  const clearInput = (event) => {
    if (event.key && event.key !== 'Enter') {
      return;
    }
    
    const inputEl = document.getElementById(inputId);
    if (inputEl) {
      inputEl.value = '';
      hasText.value = false;
      inputEl.dispatchEvent(new Event('input', { bubbles: true }));
      if (onTextChange && typeof onTextChange === 'function') {
        onTextChange('');
      }
    }
  };

  /**
   * Adds listeners to the clear icon
   */
  const addClearIconListeners = () => {
    nextTick(() => {
      const clearIconEl = document.getElementById('clear-icon-inside-input');
      if (clearIconEl) {
        clearIconEl.removeEventListener('keyup', clearInput);
        clearIconEl.removeEventListener('click', clearInput);
        
        clearIconEl.addEventListener('keyup', clearInput);
        clearIconEl.addEventListener('click', clearInput);
      }
    });
  };

  /**
   * Initializes the composable state
   */
  const initializeSpeechInput = () => {
    updateTextState();
    addClearIconListeners();
  };

  /**
   * Clears microphone error message
   */
  const clearMicrophoneError = () => {
    microphoneError.value = null;
    clearMicrophoneErrorTimer();
  };

  /**
   * Clears input and resets to initial state
   */
  const clearAndReset = () => {
    const inputEl = document.getElementById(inputId);
    if (inputEl) {
      inputEl.value = '';
      hasText.value = false;
      inputEl.dispatchEvent(new Event('input', { bubbles: true }));
      if (onTextChange && typeof onTextChange === 'function') {
        onTextChange('');
      }
    }
    
    // Reset speech recognition state
    if (isListening.value) {
      stopSpeechRecognition();
    }
    
    // Clear any previous result
    if (result.value) {
      result.value = '';
    }
    
    // Reset ALL flags
    resultProcessed.value = false;
    shouldTriggerSearch.value = false;
    successfulResult.value = false;
    microphoneError.value = null;
    clearMicrophoneErrorTimer();
  };

  watch(result, handleSpeechResult);
  
  watch(hasText, (newVal) => {
    if (newVal) {
      addClearIconListeners();
    }
  });

  watch(isListening, (newVal) => {
    if (!newVal) {
      // Only remove click outside listener when listening stops
      // Don't clear timers here - let handleSpeechResult or stopSpeechRecognition handle them
      removeClickOutsideListener();
    }
  });

  return {
    isListening,
    hasText,
    isSpeechSupported: isSupported,
    speechResult: result,
    microphoneError,
    shouldTriggerSearch,
    
    toggleSpeechRecognition,
    stopSpeechRecognition,
    handleSpeechResult,
    updateTextState,
    clearInput,
    addClearIconListeners,
    initializeSpeechInput,
    cleanSpeechText,
    clearMicrophoneError,
    clearAndReset,
    startAutoStopTimer,
    clearAutoStopTimer,
    startMaxListeningTimer,
    clearMaxListeningTimer,
    addClickOutsideListener,
    removeClickOutsideListener,
    cleanup,
    setMicrophoneError,
    clearMicrophoneErrorTimer,
    
    startSpeech: start,
    stopSpeech: stop,
  };
}