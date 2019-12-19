export const connectTo = (mapStateToProps, actions, Component) => {
    const mapDispatchToProps = dispatch => bindActionCreators(actions, dispatch)
    return connect(mapStateToProps, mapDispatchToProps)(Component)
  }
  
  const doSomeStaff = createAction()
  
  export default connectTo(
    // map state to props
    state => state.something,
    // object with actions
    { doSomeStaff },
    // component
    ({ someStaff, doSomeStaff }) => (
      <button onClick={doSomeStaff}>{someStaff}</button>
  )