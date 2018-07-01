import dotnetify from 'dotnetify';
dotnetify.hubServerUrl = 'http://localhost:5000';

/**
 * Connects React class component to the C# ViewModel
 * that shares the same name, and initializes the
 * ViewModel with the component's state object.
 *
 * @export
 * @param {*} React class component
 * @returns
 */
export default function connect(component) {
  return dotnetify.react.connect(
    component.constructor.name,
    component,
    { vmArg: component.state }
  );
}
