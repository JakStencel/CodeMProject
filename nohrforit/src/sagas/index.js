import { takeLatest } from 'redux-saga/effects'

import * as loginActions from '../actions/login'
import * as loginSagas from './login'

import * as genericActions from '../actions/generic'
import * as genericSagas from './generic'

export default function* saga() {
  const relations = [
    [loginActions, loginSagas],
    [genericActions, genericSagas],
  ]

  for (const [actions, sagas] of relations) {
    for (const [actionName, action] of Object.entries(actions)) {
      const saga = sagas[actionName]
      if (saga) yield takeLatest(action.getType(), saga)
    }
  }
}